using CustomerSupport.API.Models;
using CustomerSupport.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupport.API.ExceptionHandling
{
    // GlobalExceptionHandler provides centralized exception handling
    // for the entire ASP.NET Core Web API application.
    // Instead of writing try-catch blocks inside every controller or service,
    // exceptions can be handled here in one common place.
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        // ILogger is used to record exception details in the application logs.
        private readonly ILogger<GlobalExceptionHandler> _logger;

        // ILogger is injected through Dependency Injection.
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        // TryHandleAsync is automatically called by ASP.NET Core
        // whenever an unhandled exception occurs while processing a request.
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // These variables will store the HTTP status code
            // and user-friendly error message returned to the client.
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "An unexpected error occurred. Please try again later.";

            // This variable will store the list of errors
            IReadOnlyCollection<string> errors = [];

            // Check the type of exception and convert it
            // into an appropriate HTTP status code and message.
            switch (exception)
            {
                // Used when a validation rule is violated.
                case ApplicationValidationException validationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = validationException.Message;
                    errors = validationException.Errors;
                    break;

                // Used when a business rule is violated.
                case BusinessRuleException businessRuleException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = businessRuleException.Message;
                    break;

                // Used when the requested resource does not exist.
                case NotFoundException notFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = notFoundException.Message;
                    break;

                // Used when the request conflicts with existing data.
                case ConflictException conflictException:
                    statusCode = StatusCodes.Status409Conflict;
                    message = conflictException.Message;
                    break;

                // Occurs when Entity Framework Core detects that
                // another request has already modified the same database record.
                case DbUpdateConcurrencyException:
                    statusCode = StatusCodes.Status409Conflict;
                    message = "The data was modified by another request. Please try again.";
                    break;

                // Handles all other unexpected exceptions
                // that are not specifically handled above.
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    // Do not expose the actual exception message to the client
                    // because it may contain sensitive implementation details.
                    message = "An unexpected error occurred while processing the request.";
                    break;
            }

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                // Server-side errors are logged as Error because
                // they normally indicate an unexpected application problem.

                _logger.LogError(
                    exception,
                    "Unhandled exception occurred. Method: {Method}, Path: {Path}, TraceId: {TraceId}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    httpContext.TraceIdentifier);
            }
            else
            {
                // Known or expected failures are logged as Warning.
                _logger.LogWarning(
                    exception,
                    "Request failed with status code {StatusCode}. Method: {Method}, Path: {Path}, TraceId: {TraceId}",
                    statusCode,
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    httpContext.TraceIdentifier);
            }

            // Set the HTTP status code that will be returned to the client.
            httpContext.Response.StatusCode = statusCode;

            // Create a standard API failure response.
            // TraceIdentifier uniquely identifies the current HTTP request
            // and is useful when troubleshooting errors using application logs.
            var response = ApiResponse<object?>.FailureResponse(
                message,
                errors,
                httpContext.TraceIdentifier);

            // Convert the ApiResponse object into JSON
            // and write it to the HTTP response body.
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            // Returning true tells ASP.NET Core that
            // this exception has been successfully handled.
            return true;
        }
    }
}

