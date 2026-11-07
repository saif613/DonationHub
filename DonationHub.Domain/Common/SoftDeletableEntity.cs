using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common;

public abstract class SoftDeletableEntity : AuditableEntity, ISoftDeletable
{
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public void MarkAsDeleted(DateTime utcNow, Guid? userId)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAtUtc = utcNow;
        DeletedBy = userId;        
    }

    public void MarkAsRestored(DateTime utcNow, Guid? userId)
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAtUtc = null;
        DeletedBy = null;

        MarkAsUpdated(utcNow, userId);
    }
}
