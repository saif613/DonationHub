using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Commands.CampaignCancelled
{
    public sealed class CampaignCancelledHandler : IRequestHandler<CampaignCancelledCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        private readonly ICurrentUserService _currentUser;
        private readonly ICampaignRepository _campaignRepository;

        public CampaignCancelledHandler(IUnitOfWork unitOfWork, TimeProvider timeProvider, ICurrentUserService currentUser, ICampaignRepository campaignRepository)
        {
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
            _currentUser = currentUser;
            _campaignRepository = campaignRepository;
        }

        private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;

        public async Task<Result> Handle(CampaignCancelledCommand request, CancellationToken ct)
        {
            var campaignExist = await _campaignRepository.GetByIdAsync(request.Id, ct);

            if (campaignExist is null)
                return Errors.Campaign.NotFound;

            var isCampaignActive = await _campaignRepository.IsCampaignActive(request.Id, ct);

            if (!isCampaignActive)
                return Errors.Campaign.NotActive;

            var result = campaignExist.Cancelled(GetUtcNow(), _currentUser.UserId);

            if (result.IsFailure)
                return result;

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
