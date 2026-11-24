
namespace DonationHub.Domain.Common.Events
{
    public record CampaignCompletedEvent(Guid CampaignId, decimal TotalRaised) : IDomainEvent;
}
