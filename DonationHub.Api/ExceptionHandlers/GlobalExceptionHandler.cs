using DonationHub.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

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

            if(exception is OperationCanceledException)
            {
                httpContext.Response.StatusCode = 499;
                return true;
            }

            var (statusCode, errorCode) = exception is AppException appException ? 
                (appException.StatusCode, appException.ErrorCode) :
                (StatusCodes.Status500InternalServerError, "General.InternalServerError");

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Server error. ErrorCode: {ErrorCode}", errorCode);
            }
            else
            {
                _logger.LogWarning("Request failed. ErrorCode: {ErrorCode}", errorCode);
            }


            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(new
            {
                ErrorCode = errorCode
            }, cancellationToken);

            return true;
        }
    }
}
