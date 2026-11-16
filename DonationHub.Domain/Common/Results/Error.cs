using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common.Results
{
    public sealed record Error(string Code, string Message, ErrorType Type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    }
}
