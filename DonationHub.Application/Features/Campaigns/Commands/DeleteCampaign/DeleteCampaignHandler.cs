using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Commands.DeleteCampaign
{
    public sealed class DeleteCampaignHandler : IRequestHandler<DeleteCampaignCommand, Result>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCampaignHandler(ICampaignRepository campaignRepository, IUnitOfWork unitOfWork)
        {
            _campaignRepository = campaignRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(DeleteCampaignCommand request, CancellationToken ct)
        {
            var campaign = await _campaignRepository.GetByIdAsync(request.Id, ct);

            if (campaign is null)
                return Errors.Campaign.NotFound;

            var isCampaignHasDonation = await _campaignRepository.IsCampaignHasDonation(request.Id, ct);

            if (isCampaignHasDonation)
                _campaignRepository.Delete(campaign);

           await _campaignRepository.HardDeleteAsync(request.Id, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
