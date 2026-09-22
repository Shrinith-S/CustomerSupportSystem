using CustomerSupport.Application.DTOs.Auth;
using CustomerSupport.Application.Exceptions;
using CustomerSupport.Application.Interfaces.Authentication;
using CustomerSupport.Application.Interfaces.Repositories;
using CustomerSupport.Application.Interfaces.Services;
using CustomerSupport.Application.Mappings;
using CustomerSupport.Domain.Entities;
using CustomerSupport.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CustomerSupport.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        // Used to retrieve, validate, and create User records.
        private readonly IUserRepository _userRepository;

        // Used to store and retrieve Refresh Token records.
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        // Provides password hashing and password verification functionality.
        private readonly IPasswordService _passwordService;

        // Provides Access Token creation, Refresh Token creation, and Refresh Token hashing.
        private readonly ITokenService _tokenService;

        // Used to record important authentication events
        private readonly ILogger<AuthService> _logger;

        // All dependencies are provided through Dependency Injection.
        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordService passwordService,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request, string? ipAddress)
        {
            // Normalize the email before checking or storing it.
            // This prevents values such as User@Example.com and
            // user@example.com from being treated as different accounts.
            var email = request.Email.Trim().ToLowerInvariant();

            _logger.LogInformation("Registration started for Email: {Email}", email);

            // Email must be unique.
            // A duplicate email conflicts with existing application data,
            // so ConflictException will eventually become HTTP 409
            // through the Global Exception Handler.
            if (await _userRepository.EmailExistsAsync(email))
            {
                _logger.LogWarning("Registration rejected because the email already exists. Email: {Email}", email);
                throw new ConflictException("A user with this email address already exists.");
            }

            // Phone number is optional, therefore start with null.
            string? phoneNumber = null;

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                // Remove unnecessary spaces before checking and storing the phone number.
                phoneNumber = request.PhoneNumber.Trim();

                // When a phone number is supplied, it must also be unique.
                if (await _userRepository.PhoneNumberExistsAsync(phoneNumber))
                {
                    _logger.LogWarning("Registration rejected because the phone number already exists.");

                    throw new ConflictException("A user with this phone number already exists.");
                }
            }

            // Create the new User from values that the client is actually allowed to control.
            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                PhoneNumber = phoneNumber,

                // Public self-registration always creates a Customer.
                // The client must never be allowed to register itself
                // as a Support Executive or Administrator.
                RoleId = RoleType.Customer,

                // Newly registered users are active by default.
                IsActive = true
            };

            // Never store the plain-text password in the database.
            // PasswordService converts it into a secure password hash.
            user.PasswordHash = _passwordService.HashPassword(user, request.Password);

            // Add the new User to the DbContext through the Repository.
            await _userRepository.AddAsync(user);

            // Save first so that database-generated values such as User.Id
            // are available before creating the Refresh Token record.
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User registration completed successfully. UserId: {UserId}", user.Id);

            // Registration also signs the newly created Customer in.
            // Therefore, generate an Access Token and Refresh Token
            // and persist the Refresh Token hash.
            return await CreateAuthenticationResponseAsync(user, ipAddress);
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, string? ipAddress)
        {
            // Normalize the email exactly as we did during registration.
            var email = request.Email.Trim().ToLowerInvariant();

            _logger.LogInformation("Login attempt received for Email: {Email}", email);

            // Retrieve the User including the information required
            // for authentication and Access Token creation.
            var user = await _userRepository.GetByEmailAsync(email);

            // Use the same failure message when:
            // 1. The email does not exist.
            // 2. The account is inactive.
            // This prevents the API from revealing whether
            // a particular email address is registered.
            if (user is null || !user.IsActive)
            {
                _logger.LogWarning("Login rejected for Email: {Email}", email);

                throw new UnauthorizedException("Invalid email or password.");
            }

            // Verify the password entered by the user against
            // the secure PasswordHash stored in the database.
            var validPassword = _passwordService.VerifyPassword(user, user.PasswordHash, request.Password);

            // Again use the same generic authentication error rather than revealing
            // that the email was correct but the password was incorrect.
            if (!validPassword)
            {
                _logger.LogWarning("Login rejected for Email: {Email}", email);

                throw new UnauthorizedException("Invalid email or password.");
            }

            _logger.LogInformation("Login successful. UserId: {UserId}", user.Id);

            // Authentication succeeded.
            // Generate a fresh Access Token and Refresh Token
            // and persist only the Refresh Token hash.
            return await CreateAuthenticationResponseAsync(user, ipAddress);
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request, string? ipAddress)
        {
            _logger.LogInformation("Refresh Token rotation requested.");

            // The client sends the raw Refresh Token.
            // However, the database stores only its hash.
            // Therefore, hash the received token first
            // so that we can locate the corresponding database record.
            var tokenHash = _tokenService.ComputeRefreshTokenHash(request.RefreshToken);

            var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            // A Refresh Token cannot be used when:
            // 1. It does not exist.
            // 2. It has already been revoked.
            // 3. It has expired.
            // 4. The associated User account is inactive.
            // All of these conditions are treated as authentication failures.
            if (existingToken is null ||
                existingToken.RevokedAt.HasValue ||
                existingToken.ExpiresAt <= DateTime.UtcNow ||
                !existingToken.User.IsActive)
            {
                _logger.LogWarning("Refresh Token rotation rejected because the token is invalid, expired, revoked, or the user is inactive.");

                // Keep the external message generic.
                // Do not tell the client whether the token
                // was missing, expired, revoked, or otherwise invalid.
                throw new UnauthorizedException("Invalid or expired refresh token.");
            }

            // The existing Refresh Token is valid.
            // Generate a new short-lived Access Token.
            var newAccessToken = _tokenService.CreateAccessToken(existingToken.User);

            // Generate a completely new Refresh Token.
            // We do not continue using the existing Refresh Token.
            var newRefreshToken = _tokenService.CreateRefreshToken();

            // The hash of the new Refresh Token will be stored in SQL Server.
            var newRefreshTokenHash = _tokenService.ComputeRefreshTokenHash(newRefreshToken.Token);

            // Use one UTC timestamp for the entire rotation operation
            // so that all related audit fields remain consistent.
            var now = DateTime.UtcNow;

            // Refresh Token rotation means:
            // 1. Revoke the currently used Refresh Token.
            // 2. Link it to the hash of its replacement.
            // 3. Create a completely new Refresh Token record.
            // This helps prevent replay of previously used tokens.
            existingToken.RevokedAt = now;
            existingToken.RevokedByIp = ipAddress;
            existingToken.ReplacedByTokenHash = newRefreshTokenHash;
            existingToken.ReasonRevoked = "Replaced by a new refresh token.";

            // Maintain audit information for the token update.
            existingToken.UpdatedAt = now;
            existingToken.UpdatedBy = existingToken.UserId;

            // Create the database record for the replacement Refresh Token.
            // Notice that TokenHash is stored instead of
            // the raw Refresh Token returned to the client.
            var refreshTokenEntity = new RefreshToken
            {
                UserId = existingToken.UserId,
                TokenHash = newRefreshTokenHash,
                ExpiresAt = newRefreshToken.ExpiresAt,
                CreatedByIp = ipAddress,

                // The authenticated User owns this Refresh Token.
                CreatedBy = existingToken.UserId
            };

            // Add the replacement Refresh Token.
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            // Save both changes together:
            // 1. Revocation of the old Refresh Token.
            // 2. Creation of the new Refresh Token.
            await _refreshTokenRepository.SaveChangesAsync();

            _logger.LogInformation("Refresh Token rotation completed successfully. UserId: {UserId}", existingToken.UserId);

            // Return the raw new tokens only to the client.
            // IMPORTANT:
            // Never write the Access Token, Refresh Token, or Refresh Token hash to application logs.
            return new AuthResponseDTO
            {
                User = existingToken.User.ToResponseDTO(),
                AccessToken = newAccessToken.Token,
                AccessTokenExpiresAt = newAccessToken.ExpiresAt,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
            };
        }

        public async Task RevokeRefreshTokenAsync(RefreshTokenRequestDTO request, string? ipAddress)
        {
            _logger.LogInformation("Logout requested.");

            // Hash the raw token received from the client before searching the database.
            var tokenHash = _tokenService.ComputeRefreshTokenHash(request.RefreshToken);

            var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            // Logout requires a currently valid, non-revoked Refresh Token.
            // Returning a generic error avoids exposing internal token state to the client.
            if (existingToken is null || existingToken.RevokedAt.HasValue)
            {
                _logger.LogWarning("Logout rejected because the Refresh Token is invalid or already revoked.");

                throw new UnauthorizedException("Invalid or already revoked refresh token.");
            }

            // Revoking the Refresh Token prevents it from being used
            // later to obtain another Access Token.
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.RevokedByIp = ipAddress;
            existingToken.ReasonRevoked = "User logged out.";

            // Update audit information for the token record.
            existingToken.UpdatedAt = DateTime.UtcNow;
            existingToken.UpdatedBy = existingToken.UserId;

            // Persist the revocation.
            await _refreshTokenRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Logout completed successfully. UserId: {UserId}",
                existingToken.UserId);
        }

        // This helper method is used after successful registration and successful login.
        // Its responsibility is to:
        // 1. Create an Access Token.
        // 2. Create a Refresh Token.
        // 3. Store only the Refresh Token hash.
        // 4. Return the raw tokens to the client.
        private async Task<AuthResponseDTO> CreateAuthenticationResponseAsync(User user, string? ipAddress)
        {
            // Create the short-lived JWT Access Token.
            var accessToken = _tokenService.CreateAccessToken(user);

            // Create a longer-lived Refresh Token.
            var refreshToken = _tokenService.CreateRefreshToken();

            // Never store the raw Refresh Token in SQL Server.
            // Store only its one-way hash.
            var tokenHash = _tokenService.ComputeRefreshTokenHash(refreshToken.Token);

            // Create the database representation of the Refresh Token.
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = refreshToken.ExpiresAt,

                // Store the originating IP address for auditing
                // and security-related investigation.
                CreatedByIp = ipAddress,
                CreatedBy = user.Id
            };

            // Add the Refresh Token record to the database.
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            await _refreshTokenRepository.SaveChangesAsync();

            // Return the raw Access Token and Refresh Token to the client.
            // The raw Refresh Token exists only in the response;
            // SQL Server contains only its hash.
            return new AuthResponseDTO
            {
                User = user.ToResponseDTO(),
                AccessToken = accessToken.Token,
                AccessTokenExpiresAt = accessToken.ExpiresAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt
            };
        }
    }
}
