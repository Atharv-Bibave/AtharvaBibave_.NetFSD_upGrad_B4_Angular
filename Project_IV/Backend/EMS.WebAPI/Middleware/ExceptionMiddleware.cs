using System.Net;
using System.Text.Json;
using EMS.WebAPI.DTOs;

namespace EMS.WebAPI.Middleware
{
    /// <summary>
    /// Centralized exception handler. Maps typed exceptions to HTTP status codes
    /// and always returns a standardized ApiResponse envelope.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                ArgumentNullException or ArgumentException
                    => (HttpStatusCode.BadRequest,      "Invalid request data."),

                UnauthorizedAccessException
                    => (HttpStatusCode.Unauthorized,    "Unauthorized access."),

                KeyNotFoundException
                    => (HttpStatusCode.NotFound,        "The requested resource was not found."),

                InvalidOperationException
                    => (HttpStatusCode.Conflict,        "Operation could not be completed due to a conflict."),

                NotSupportedException
                    => (HttpStatusCode.BadRequest,      "The operation is not supported."),

                TimeoutException
                    => (HttpStatusCode.GatewayTimeout,  "The request timed out."),

                _   => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse.Fail(
                message: message,
                errors:  new[] { ex.Message });

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response, JsonOptions));
        }
    }
}
