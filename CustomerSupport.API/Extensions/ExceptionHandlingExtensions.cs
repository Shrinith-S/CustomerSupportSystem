using CustomerSupport.API.ExceptionHandling;

namespace CustomerSupport.API.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        // Extension method for IServiceCollection.
        // This allows us to register exception handling in Program.cs
        // using a clean statement such as:
        // builder.Services.AddGlobalExceptionHandling();
        public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
        {
            // Register GlobalExceptionHandler as the application's centralized exception handler.
            // Whenever an unhandled exception occurs during request processing,
            // ASP.NET Core can pass that exception to GlobalExceptionHandler.
            services.AddExceptionHandler<GlobalExceptionHandler>();

            // Registers ASP.NET Core ProblemDetails services.
            // This also provides the required configuration
            // for UseExceptionHandler().
            services.AddProblemDetails();

            // Return IServiceCollection so that additional services
            // can be registered using method chaining.
            return services;
        }
    }
}
