using DonationHub.Application.Exceptions;
using DonationHub.Domain.Common.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            if (exception is ConcurrencyException)
            {
                _logger.LogWarning(exception, "Concurrency conflict occurred. TraceId: {TraceId}", traceId);

                var error = Errors.Campaign.ConcurrencyConflict;

                var concurrencyProblemDetails = new ProblemDetails
                {
                    Type = "https://httpstatuses.io/409",
                    Title = error.Message,
                    Status = StatusCodes.Status409Conflict,
                    Instance = httpContext.Request.Path
                };

                concurrencyProblemDetails.Extensions["code"] = error.Code;
                concurrencyProblemDetails.Extensions["traceId"] = traceId;

                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                httpContext.Response.ContentType = "application/problem+json";

                await httpContext.Response.WriteAsJsonAsync(concurrencyProblemDetails, cancellationToken);

                return true;
            }

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
