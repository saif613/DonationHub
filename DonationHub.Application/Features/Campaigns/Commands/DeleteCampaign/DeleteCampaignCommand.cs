using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Commands.DeleteCampaign
{
    public record DeleteCampaignCommand(Guid Id) : IRequest<Result>;
    
}
