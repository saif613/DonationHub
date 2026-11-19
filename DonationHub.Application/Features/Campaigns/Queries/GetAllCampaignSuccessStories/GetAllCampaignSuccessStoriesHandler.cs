using DonationHub.Application.Features.Campaigns.DTOs.Response;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Queries.GetAllCampaignSuccessStories
{
    public sealed class GetAllCampaignSuccessStoriesHandler : IRequestHandler<GetAllCampaignSuccessStoriesQuery, Result<IReadOnlyList<CampaignListItemResponseDto>>>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly TimeProvider _timeProvider;

        public GetAllCampaignSuccessStoriesHandler(ICampaignRepository campaignRepository, TimeProvider timeProvider)
        {
            _campaignRepository = campaignRepository;
            _timeProvider = timeProvider;
        }

        private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
        public async Task<Result<IReadOnlyList<CampaignListItemResponseDto>>> Handle(GetAllCampaignSuccessStoriesQuery request, CancellationToken ct)
        {
            var campaigns = await _campaignRepository.GetAllCompletedAsync(ct);

            var isArabic = request.Language == "ar";

            var utcNow = GetUtcNow();

            var response = campaigns.Select(c =>
            {
                var progressPercent = Math.Min(
                    100m,
                    (c.RaisedAmount / c.GoalAmount.Amount) * 100m);

                var daysLeft = Math.Max(
                    0,
                    (c.EndDate.Date - utcNow.Date).Days);

                return new CampaignListItemResponseDto(
                    c.Id,
                    isArabic ? c.TitleAr : c.TitleEn,
                    isArabic ? c.DescriptionAr : c.DescriptionEn,
                    c.GoalAmount.Amount,
                    c.GoalAmount.Currency,
                    c.RaisedAmount,
                    c.EndDate,
                    c.CategoryId,
                    c.Status,
                    c.BannerFileName,
                    progressPercent,
                    daysLeft
                );
            }).ToList();

            return Result.Success<IReadOnlyList<CampaignListItemResponseDto>>(response);
        }
    }
}
