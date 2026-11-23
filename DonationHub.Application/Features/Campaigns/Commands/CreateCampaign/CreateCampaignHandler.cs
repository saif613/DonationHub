using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Features.Campaigns.DTOs.Response;
using DonationHub.Application.Features.Campaigns.Response.DTOs;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static DonationHub.Domain.Common.Results.Errors;
using Campaign = DonationHub.Domain.Entities.Campaign;

namespace DonationHub.Application.Features.Campaigns.Commands.CreateCampaign
{    
    public sealed class CreateCampaignHandler : IRequestHandler<CreateCampaignCommand, Result<CampaignResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICampaignRepository _campaignRepository;
        private readonly TimeProvider _timeProvider;
        private readonly ICurrentUserService _currentUser;
        private readonly ICategoryRepository _categoryRepository;

        public CreateCampaignHandler(IUnitOfWork unitOfWork, ICampaignRepository campaignRepository, TimeProvider timeProvider, ICurrentUserService currentUser, ICategoryRepository categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _campaignRepository = campaignRepository;
            _timeProvider = timeProvider;
            _currentUser = currentUser;
            _categoryRepository = categoryRepository;
        }

        private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
        public async Task<Result<CampaignResponseDto>> Handle(CreateCampaignCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId is null)
                return Errors.Auth.Unauthorized;            

            var titleAr = request.TitleAr.Trim();
            var titleEn = request.TitleEn.Trim();

            var utcNow = GetUtcNow();

            if (request.EndDate <= utcNow)
                return Errors.Campaign.EndDateInPast;

            var categoryExists = await _categoryRepository.GetByIdAsync(request.CategoryId, ct);

            if (categoryExists is null)
                return Errors.Category.NotFound;

            if (await _campaignRepository.ExistsByTitleArAsync(titleAr, ct: ct))
                return Errors.Campaign.ArTitleAlreadyExists;

            if (await _campaignRepository.ExistsByTitleEnAsync(titleEn, ct: ct))
                return Errors.Campaign.EnTitleAlreadyExists;

            var money = Domain.ValueObjects.Money.Create(request.Amount, request.Currency);

            if (money.IsFailure)
                return money.Error;

            var campaign = Campaign.Create(titleAr, titleEn, request.DescriptionAr, request.DescriptionEn, money.Value, request.EndDate, request.CategoryId, utcNow, userId);

            if (campaign.IsFailure)
                return campaign.Error;

            await _campaignRepository.AddAsync(campaign.Value, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var response = new CampaignResponseDto(campaign.Value.Id , campaign.Value.TitleAr, campaign.Value.TitleEn, campaign.Value.DescriptionAr, campaign.Value.DescriptionEn, campaign.Value.GoalAmount.Amount, campaign.Value.GoalAmount.Currency, campaign.Value.EndDate, campaign.Value.CategoryId);

            return Result.Success(response);
        }
    }
}
