using System;
using System.Collections.Generic;
using System.Text;

namespace Donation.Tests.Common
{
    public static class TestDateTime
    {
        public static readonly DateTime Now = new(2026, 11, 5, 10, 0, 0, DateTimeKind.Utc);

        public static readonly DateTime Later = new(2026, 11, 6, 10, 0, 0, DateTimeKind.Utc);
    }
}
