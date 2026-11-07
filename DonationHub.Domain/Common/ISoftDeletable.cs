using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAtUtc { get; }
    Guid? DeletedBy { get; }
    void MarkAsDeleted(DateTime utcNow, Guid? userId);
    void MarkAsRestored(DateTime utcNow, Guid? userId);
}
