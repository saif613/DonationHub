using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Commands.CampaignCancelled
{
    public record CampaignCancelledCommand(Guid Id) : IRequest<Result>;

}
