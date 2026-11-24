
using DonationHub.Domain.Common.Events;

namespace DonationHub.Application.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default);
    }
}
