using DonationHub.Application.Features.Campaigns.DTOs.Response;
using DonationHub.Application.Features.Campaigns.Response.DTOs;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Commands.CreateCampaign
{
    public record CreateCampaignCommand(
        string TitleAr,
        string TitleEn,
        string DescriptionAr,
        string DescriptionEn,
        decimal Amount,
        string Currency,
        DateTime EndDate,
        Guid CategoryId
        ): IRequest<Result<CampaignResponseDto>>;
}
