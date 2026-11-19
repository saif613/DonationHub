using DonationHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.DTOs.Response
{
    public record CampaignDetailsResponseDto(
    Guid Id,
    string Title,
    string Description,
    decimal GoalAmount,
    string Currency,
    decimal RaisedAmount,
    DateTime EndDate,
    Guid CategoryId,
    CampaignStatus Status,
    string? BannerFileName,
    decimal ProgressPercent,
    int DaysLeft,
    byte[] RowVersion
);
}
