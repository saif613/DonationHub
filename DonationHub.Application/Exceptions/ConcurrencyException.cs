using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Exceptions
{
    public class ConcurrencyException: Exception
    {
        public ConcurrencyException(Exception? inner = null): base("A concurrency conflict occurred.", inner) { }
    }
}
