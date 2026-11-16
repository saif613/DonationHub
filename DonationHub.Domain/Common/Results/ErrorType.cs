using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common.Results
{
    public enum ErrorType
    {
        Validation,
        Unauthorized,
        Forbidden,
        NotFound,
        Conflict,
        PayloadTooLarge,
        UnprocessableEntity,
        TooManyRequests,
        Failure,
    }
}
