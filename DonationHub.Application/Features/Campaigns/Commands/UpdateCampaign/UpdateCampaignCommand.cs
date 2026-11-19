using DonationHub.Application.Features.Campaigns.DTOs.Response;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign
{
    public record UpdateCampaignCommand(
    Guid Id,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    decimal Amount,
    string Currency,
    DateTime EndDate,
    Guid CategoryId,
    byte[] RowVersion) : IRequest<Result<UpdateResponseDto>>;
    
}
