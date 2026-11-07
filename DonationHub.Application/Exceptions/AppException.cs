using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Exceptions
{
    public class AppException : Exception
    {
        public string ErrorCode { get; } = null!;
        public int StatusCode { get; }

        public AppException(string errorCode, int statusCode) : base(errorCode)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

    }
}
