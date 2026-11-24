using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Events
{
    public record CampaignCompletedNotification(Guid CampaignId, decimal TotalRaised) : INotification;
}
