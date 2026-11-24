using DonationHub.Application.Events;
using DonationHub.Domain.Common.Events;
using MediatR;

namespace DonationHub.Infrastructure.Events
{
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IPublisher _publisher;

        public DomainEventDispatcher(IPublisher publisher)
        {
            _publisher = publisher;
        }
        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default)
        {
            foreach(var domainEvent in domainEvents)
            {
                switch (domainEvent)
                {
                    case CampaignCompletedEvent e:
                        await _publisher.Publish(new CampaignCompletedNotification(e.CampaignId, e.TotalRaised), ct);
                    break;
                }
            }
        }
    }
}
