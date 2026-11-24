using DonationHub.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common;

public abstract class BaseEntity 
{
    public Guid Id { get; protected set; }
    private readonly List<IDomainEvent> _domainEvents = [];
    public  IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
