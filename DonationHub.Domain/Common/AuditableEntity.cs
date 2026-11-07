using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    
    protected void MarkAsUpdated(DateTime utcNow, Guid? userId)
    {
        UpdatedAt = utcNow;
        UpdatedBy = userId;
    }
}
