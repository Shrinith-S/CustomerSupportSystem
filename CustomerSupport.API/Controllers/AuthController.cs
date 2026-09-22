using CustomerSupport.API.Models;
using CustomerSupport.Application.DTOs.Auth;
using CustomerSupport.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupport.API.Controllers
{
    [ApiController]

    // All endpoints inside this Controller will start with: api/auth
    // Examples:
    // POST api/auth/register
    // POST api/auth/login
    // POST api/auth/refresh-token
    // POST api/auth/logout
    [Route("api/auth")]

    // These endpoints are allowed to execute without JWT authentication.
    [AllowAnonymous]
    public sealed class AuthController : ControllerBase
    {
        // The Controller depends only on the IAuthService abstraction.
        private readonly IAuthService _authService;

        // IAuthService is provided through Dependency Injection.
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        // Creates a new Customer account and, after successful registration,
        // returns an Access Token and Refresh Token.
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register(RegisterRequestDTO request)
        {
            // Delegate the registration workflow to AuthService.
            // AuthService is responsible for:
            // - Normalizing the email
            // - Checking duplicate email/phone number
            // - Creating the User
            // - Hashing the password
            // - Saving the User
            // - Creating authentication tokens

            // The current request IP address is also passed so that
            // Refresh Token creation can maintain audit information.
            var result = await _authService.RegisterAsync(request, GetIpAddress());

            // We do not catch ConflictException here.
            // For example, if the email or phone number already exists,
            // AuthService throws ConflictException.

            // GlobalExceptionHandler converts that exception into: HTTP 409 Conflict
            // This keeps the Controller free from repetitive try-catch and error-response code.

            // Wrap the successful authentication result inside ApiResponse<T> structure.
            var response = ApiResponse<AuthResponseDTO>.SuccessResponse(
                    result,
                    "Registration successful.");

            return Ok(response);
        }

        // POST: api/auth/login
        // Authenticates a User using email and password.
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login(LoginRequestDTO request)
        {
            // Pass the login request to AuthService.
            // AuthService validates:
            // - Whether the User exists
            // - Whether the account is active
            // - Whether the password is correct

            // If authentication succeeds, it generates a new Access Token and Refresh Token.
            var result = await _authService.LoginAsync(request, GetIpAddress());

            // Create the standard successful API response.
            var response = ApiResponse<AuthResponseDTO>.SuccessResponse(
                    result,
                    "Login successful.");

            // Successful login returns HTTP 200 OK.
            return Ok(response);
        }

        // POST: api/auth/refresh-token
        // Uses a valid Refresh Token to obtain:
        // - A new Access Token
        // - A new Refresh Token
        // The old Refresh Token is revoked as part of token rotation.
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> RefreshToken(RefreshTokenRequestDTO request)
        {
            // Delegate Refresh Token validation and rotation to the Authentication Service.
            // AuthService will:
            // - Hash the supplied raw Refresh Token
            // - Find the corresponding database record
            // - Validate that it is not revoked or expired
            // - Validate that the associated User is active
            // - Revoke the existing Refresh Token
            // - Generate a new Access Token
            // - Generate and store a new Refresh Token hash
            var result = await _authService.RefreshTokenAsync(request, GetIpAddress());

            // Wrap the newly generated authentication tokens
            // inside our common response structure.
            var response = ApiResponse<AuthResponseDTO>.SuccessResponse(
                    result,
                    "Token refreshed successfully.");

            return Ok(response);
        }

        // POST: api/auth/logout
        // Logs the User out by revoking the supplied Refresh Token.
        // Note:
        // Revoking a Refresh Token does not immediately invalidate
        // an Access Token that has already been issued.
        // The Access Token remains valid until its expiration time.
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object?>>> Logout(RefreshTokenRequestDTO request)
        {
            // Ask AuthService to revoke the Refresh Token.
            // AuthService will:
            // - Hash the supplied raw Refresh Token
            // - Find the stored token record
            // - Mark it as revoked
            // - Store the IP address and audit information
            // After revocation, this Refresh Token can no longer
            // be used to generate another Access Token.
            await _authService.RevokeRefreshTokenAsync(request, GetIpAddress());

            // Logout does not need to return any business data,
            // so Data is null.
            var response = ApiResponse<object?>.SuccessResponse(
                    null,
                    "Logout successful.");

            return Ok(response);
        }

        // Returns the IP address of the client making the current request.
        // We pass this value to AuthService so Refresh Token records
        // can maintain audit information such as:
        // - CreatedByIp
        // - RevokedByIp
        private string? GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
