using DonationHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Response.DTOs
{
    public record CampaignResponseDto(
        Guid Id,
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        decimal Amount,
        string Currency,
        DateTime endDate,
        Guid categoryId
        );

}
