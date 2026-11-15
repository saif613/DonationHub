using DonationHub.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Diagnostics;

namespace DonationHub.Api.Extensions
{
    public static class ResultExtensions
    {
        public static ObjectResult ToProblem(this Result result, HttpContext httpContext)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot convert a successful result to ProblemDetails");

            var error = result.Error;
            var statusCode = MapStatusCode(error.Type);

            ProblemDetails problem = result.ValidationErrors.Count > 0 ? new ValidationProblemDetails(new Dictionary<string, string[]>(result.ValidationErrors)) : new ProblemDetails();

            problem.Type = $"https://httpstatuses.io/{statusCode}";
            problem.Title = error.Message;
            problem.Status = statusCode;
            problem.Instance = httpContext.Request.Path;
            problem.Extensions["code"] = error.Code;
            problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            return new ObjectResult(problem)
            {
                StatusCode = statusCode,
                ContentTypes = { "application/problem+json" }
            };
        }

        public static int MapStatusCode(ErrorType type) => type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.PayloadTooLarge => StatusCodes.Status413PayloadTooLarge,
            ErrorType.UnprocessableEntity => StatusCodes.Status422UnprocessableEntity,
            ErrorType.TooManyRequests => StatusCodes.Status429TooManyRequests,
            _   => StatusCodes.Status500InternalServerError
        };
    }
}
