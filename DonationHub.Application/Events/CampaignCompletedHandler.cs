using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Events
{
    public sealed class CampaignCompletedHandler : INotificationHandler<CampaignCompletedNotification>
    {
        public Task Handle(CampaignCompletedNotification notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
