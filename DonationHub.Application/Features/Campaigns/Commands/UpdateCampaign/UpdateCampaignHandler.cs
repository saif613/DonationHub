using DonationHub.Application.Exceptions;
using DonationHub.Application.Features.Campaigns.DTOs.Response;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static DonationHub.Domain.Common.Results.Errors;

namespace DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign
{
    public sealed class UpdateCampaignHandler : IRequestHandler<UpdateCampaignCommand, Result<UpdateResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICampaignRepository _campaignRepository;
        private readonly TimeProvider _timeProvider;
        private readonly ICurrentUserService _currentUser;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCampaignHandler(IUnitOfWork unitOfWork, ICampaignRepository campaignRepository, TimeProvider timeProvider, ICurrentUserService currentUser, ICategoryRepository categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _campaignRepository = campaignRepository;
            _timeProvider = timeProvider;
            _currentUser = currentUser;
            _categoryRepository = categoryRepository;
        }

        private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
        public async Task<Result<UpdateResponseDto>> Handle(UpdateCampaignCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            if (userId is null)
                return Errors.Auth.Unauthorized;

            var titleAr = request.TitleAr.Trim();
            var titleEn = request.TitleEn.Trim();

            var utcNow = GetUtcNow();

            if (request.EndDate <= utcNow)
                return Errors.Campaign.EndDateInPast;

            var campaignExists = await _campaignRepository.GetByIdAsync(request.Id, ct);

            if (campaignExists is null)
                return Errors.Campaign.NotFound;

            var categoryExists = await _categoryRepository.GetByIdAsync(request.CategoryId, ct);

            if (categoryExists is null)
                return Errors.Category.NotFound;

            if (await _campaignRepository.ExistsByTitleArAsync(titleAr, request.Id, ct: ct))
                return Errors.Campaign.ArTitleAlreadyExists;

            if (await _campaignRepository.ExistsByTitleEnAsync(titleEn, request.Id, ct: ct))
                return Errors.Campaign.EnTitleAlreadyExists;

            if (request.Amount < campaignExists.RaisedAmount)
                return Errors.Campaign.GoalBelowRaised;

            if (campaignExists.Status != CampaignStatus.Active)
                return Errors.Campaign.NotActive;

            var newMoney = Money.Create(request.Amount, request.Currency);

            _campaignRepository.SetOriginalRowVersion(campaignExists, request.RowVersion);

            campaignExists.UpdateDetails(titleAr, titleEn, request.DescriptionAr, request.DescriptionEn, request.CategoryId, utcNow, userId);

            campaignExists.ChangeGoal(newMoney, utcNow, userId);

            campaignExists.ChangeEndDate(request.EndDate, utcNow, userId);

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
            }

            catch (ConcurrencyException)
            {
                return Errors.Campaign.ConcurrencyConflict;
            }


            var response = new UpdateResponseDto(campaignExists.Id, campaignExists.TitleAr, campaignExists.TitleEn, campaignExists.DescriptionAr,
            campaignExists.DescriptionEn,
            campaignExists.GoalAmount.Amount,
            campaignExists.GoalAmount.Currency,
            campaignExists.RaisedAmount,
            campaignExists.EndDate,
            campaignExists.CategoryId,
            campaignExists.RowVersion);

            return Result.Success(response);
        }
    }
}
