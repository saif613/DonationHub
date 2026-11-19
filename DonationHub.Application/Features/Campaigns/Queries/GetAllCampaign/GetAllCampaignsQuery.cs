using DonationHub.Application.Features.Campaigns.DTOs.Response;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Queries.GetAllCampaign
{
    public record GetAllCampaignsQuery(string Language): IRequest<Result<IReadOnlyList<CampaignListItemResponseDto>>>;
    
}
