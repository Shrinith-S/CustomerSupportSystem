using CustomerSupport.API.Models;
using CustomerSupport.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace CustomerSupport.API.Extensions
{
    // It contains extension methods related to JWT Authentication.
    // Keeping authentication configuration here prevents Program.cs
    // from becoming overloaded with JWT-specific setup code.
    public static class AuthenticationExtensions
    {
        // Registers JWT Bearer Authentication with the
        // ASP.NET Core Dependency Injection container.

        // IConfiguration is used to read JWT settings
        // from appsettings.json or another configuration source.
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Read the JWT configuration values and create
            // a strongly typed JwtSettings object.

            // These settings will be used both when:
            // 1. Creating JWT Access Tokens.
            // 2. Validating JWT Access Tokens received from clients.

            var jwtSettings = new JwtSettings
            {
                // Secret key used to digitally sign and validate JWTs.
                // Authentication cannot work safely without this value.    
                Key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured."),

                // Issuer identifies the application/server that creates the JWT.
                Issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured."),

                // Audience identifies the intended recipient or consumer of the JWT.
                Audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured."),

                // Determines how long the JWT Access Token remains valid.
                // Use 30 minutes when the setting is not explicitly configured.
                AccessTokenMinutes = int.Parse(configuration["Jwt:AccessTokenMinutes"] ?? "30"),

                // Determines how long the Refresh Token remains valid.
                // Use 7 days when the setting is not explicitly configured.
                RefreshTokenDays = int.Parse(configuration["Jwt:RefreshTokenDays"] ?? "7"),

                // Read the allowed clock-skew/grace period.
                // If it is not configured, use 2 minutes.
                ClockSkewMinutes = int.Parse(configuration["Jwt:ClockSkewMinutes"] ?? "2"),
            };

            // The signing key must contain enough bytes to provide
            // sufficient security for the HMAC-SHA256 signing algorithm.
            // We validate this during application startup so that
            // an invalid or weak key is discovered immediately.
            if (Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
            {
                throw new InvalidOperationException("Jwt:Key must contain at least 32 bytes.");
            }

            // Register the JwtSettings object as a Singleton.
            // The settings do not change while the application is running,
            // so one shared instance is sufficient.
            // JwtTokenService can now receive JwtSettings through constructor Dependency Injection.
            services.AddSingleton(jwtSettings);

            // Configure ASP.NET Core Authentication.
            // JwtBearerDefaults.AuthenticationScheme tells ASP.NET Core
            // that Bearer Tokens should be used as the default authentication mechanism.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

                // Register the JWT Bearer authentication handler.
                // This handler reads the Access Token from requests such as:
                // Authorization: Bearer <access-token>
                // and validates the token before allowing the request to be treated as authenticated.
                .AddJwtBearer(options =>
                {
                    // Define the rules that every incoming JWT Access Token must satisfy.
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Verify that the token was issued by the expected application/server.
                        ValidateIssuer = true,

                        // The issuer in the JWT must match this value.
                        ValidIssuer = jwtSettings.Issuer,

                        // Verify that the token was created for the expected audience/client.
                        ValidateAudience = true,

                        // The audience in the JWT must match this value.
                        ValidAudience = jwtSettings.Audience,

                        // Verify that the token has not expired.
                        // If the token expiration time has passed, authentication will fail.
                        ValidateLifetime = true,

                        // Allow a small tolerance when validating
                        // time-based JWT claims.
                        //
                        // Example:
                        //
                        // Token ExpiresAt = 10:30 AM
                        // ClockSkew        = 2 minutes
                        //
                        // The token may continue to pass lifetime
                        // validation until approximately 10:32 AM.
                        //
                        // This tolerance helps handle small clock
                        // differences between systems.
                        ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkewMinutes),

                        // Verify the token's digital signature.
                        // This ensures that the token was generated
                        // using our secret signing key and that its
                        // contents were not modified after creation.
                        ValidateIssuerSigningKey = true,

                        // Use the same secret key that JwtTokenService
                        // uses when signing Access Tokens.
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                    };

                    // JwtBearerEvents allows us to customize what happens
                    // during different JWT authentication/authorization events.
                    // Here we customize 401 and 403 responses so they follow
                    // the same ApiResponse<T> structure used by our Controllers.
                    options.Events =
                        new JwtBearerEvents
                        {
                            // OnChallenge executes when a protected endpoint requires
                            // authentication but the request cannot be authenticated.

                            // Common examples:
                            // - Access Token is missing.
                            // - Access Token is expired.
                            // - Token signature is invalid.
                            // - Issuer is invalid.
                            // - Audience is invalid.

                            // The correct response is: HTTP 401 Unauthorized.
                            OnChallenge =
                                async context =>
                                {
                                    // Prevent ASP.NET Core from generating
                                    // its default authentication challenge response.
                                    // We will create our own JSON response instead.
                                    context.HandleResponse();

                                    // Authentication failed, therefore return 401.
                                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                                    // Inform the client that the response body is JSON.
                                    context.Response.ContentType = "application/json";

                                    // Bearer authentication commonly includes
                                    // the WWW-Authenticate response header
                                    // when returning HTTP 401.
                                    context.Response.Headers["WWW-Authenticate"] = "Bearer";

                                    // Create the same standard failure-response
                                    // structure used throughout our API.
                                    var response = ApiResponse<object?>.FailureResponse(
                                            "Authentication failed. A valid access token is required.");

                                    // Serialize the ApiResponse object to JSON
                                    // and write it to the HTTP response body.
                                    await context.Response.WriteAsJsonAsync(response);
                                },

                            // OnForbidden executes when authentication
                            // has already succeeded, but the authenticated User
                            // does not have permission to access the resource.

                            // Example:
                            // [Authorize(Roles = "Administrator")]

                            // A Customer sends a valid Access Token.
                            // The User is authenticated successfully,
                            // but does not have the Administrator Role.

                            // The correct response is: HTTP 403 Forbidden.
                            OnForbidden =
                                async context =>
                                {
                                    // Authorization failed, not authentication.
                                    // Therefore return HTTP 403 Forbidden.
                                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                                    context.Response.ContentType = "application/json";

                                    // Return the same common failure-response
                                    // format used throughout the API.
                                    var response = ApiResponse<object?>.FailureResponse(
                                            "You do not have permission to access this resource.");

                                    await context.Response.WriteAsJsonAsync(response);
                                }
                        };
                });

            // Return IServiceCollection so additional services
            // can continue to be registered using method chaining.
            return services;
        }
    }
}
