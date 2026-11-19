using DonationHub.Application.Features.Campaigns.DTOs.Response;
using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Queries.GetCampaignById
{
    public sealed class GetCampaignByIdHandler : IRequestHandler<GetCampaignByIdQuery, Result<CampaignDetailsResponseDto>>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly TimeProvider _timeProvider;

        public GetCampaignByIdHandler(ICampaignRepository campaignRepository, TimeProvider timeProvider)
        {
            _campaignRepository = campaignRepository;
            _timeProvider = timeProvider;
        }
        private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
        public async Task<Result<CampaignDetailsResponseDto>> Handle(GetCampaignByIdQuery request, CancellationToken ct)
        {          

            var campaign = await _campaignRepository.GetByIdAsync(request.Id, ct);

            if (campaign is null)
                return Errors.Campaign.NotFound;

            var isArabic = request.Language == "ar";

            var title = isArabic ? campaign.TitleAr : campaign.TitleEn;

            var description = isArabic ? campaign.DescriptionAr : campaign.DescriptionEn;

            var progressPercent = Math.Min(100m, (campaign!.RaisedAmount / campaign.GoalAmount.Amount) * 100m);

            var utcNow = GetUtcNow();

            var daysLeft = Math.Max(0, (campaign.EndDate.Date - utcNow.Date).Days);         

            var response = new CampaignDetailsResponseDto(campaign.Id, title, description, campaign.GoalAmount.Amount, campaign.GoalAmount.Currency, campaign.RaisedAmount,campaign.EndDate, campaign.CategoryId, campaign.Status, campaign.BannerFileName, progressPercent, daysLeft, campaign.RowVersion);

            return Result.Success(response);
        }
    }
}
