using CustomerSupport.API.Extensions;
using CustomerSupport.Application.Extensions;
using CustomerSupport.Infrastructure.Extensions;

namespace CustomerSupport.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Register ASP.NET Core's built-in OpenAPI document generation. 
            // This generates the OpenAPI specification that describes 
            // our API endpoints, request DTOs, response DTOs, and schemas.
            builder.Services.AddOpenApi();

            var connectionString =
                builder.Configuration.GetConnectionString("CustomerSupportDBConnection")
                ?? throw new InvalidOperationException("CustomerSupportDBConnection is not configured.");

            builder.Services.AddInfrastructureServices(connectionString);

            // Register the application's global exception handler.
            // This allows unhandled exceptions from the application
            // to be processed by GlobalExceptionHandler.
            builder.Services.AddGlobalExceptionHandling();

            builder.Services.AddApplicationServices();

            // Configure JWT Bearer Authentication.
            // This method reads JWT settings from configuration and registers:
            // - JWT Bearer authentication
            // - Signing-key validation
            // - Issuer validation
            // - Audience validation
            // - Token lifetime validation
            // - Name and Role claim configuration
            // It enables ASP.NET Core to authenticate requests containing:
            // Authorization: Bearer <access-token>
            builder.Services.AddJwtAuthentication(builder.Configuration);

            var app = builder.Build();

            // Expose OpenAPI and Swagger UI only in Development.
            if (app.Environment.IsDevelopment())
            {
                // Map the generated OpenAPI JSON document.
                // The default document will be available at: /openapi/v1.json
                app.MapOpenApi();

                // Enable the interactive Swagger UI.
                // Swagger UI allows us to:
                // - View available API endpoints
                // - Inspect request and response models
                // - Send requests directly from the browser
                // - Test the API during development
                app.UseSwaggerUI(options =>
                {
                    // Tell Swagger UI where the generated
                    // OpenAPI document can be found.
                    options.SwaggerEndpoint(
                        "/openapi/v1.json",
                        "Customer Support API v1");

                    // Swagger UI will be available at: /swagger
                    options.RoutePrefix = "swagger";
                });
            }

            // Add the global exception handling middleware to the HTTP request pipeline.
            // When an unhandled exception occurs,
            // ASP.NET Core forwards it to the registered exception handler.
            app.UseExceptionHandler();


            app.UseHttpsRedirection();

            // Authenticate the current request.
            // The JWT Bearer middleware looks for:
            // Authorization: Bearer <access-token>

            // If a valid Access Token is present, ASP.NET Core creates
            // an authenticated ClaimsPrincipal and places it in:
            // HttpContext.User

            // Later components can read User ID, Name, Email, and Role
            // information from the JWT Claims.
            app.UseAuthentication();

            // Perform authorization after authentication.
            // This middleware processes attributes such as:
            // [Authorize]
            // [Authorize(Roles = "Administrator")]
            // [AllowAnonymous]

            // UseAuthorization must come after UseAuthentication
            // because authorization needs to know the authenticated User.
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
