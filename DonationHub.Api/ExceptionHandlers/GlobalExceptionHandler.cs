using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DonationHub.Api.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);

            var problemDetails = new ProblemDetails
            {
                Type = "https://httpstatuses.io/500",
                Title = "An unexpected error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["code"] = "General.InternalServerError";
            problemDetails.Extensions["traceId"] = traceId;

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
