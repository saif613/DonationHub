using DonationHub.Application.Exceptions;
using DonationHub.Application.Features.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Application.UnitTests.Common.TestData;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DonationHub.Application.UnitTests.Features.Campaigns.Commands.UpdateCampaign
{
    public class UpdateCampaignHandlerTests
    {
        [Fact]
        public async Task Handle_WhenCampaignNotFound_ShouldReturnCampaignNotFound()
        {
            var data = new CampaignTestData();

            var unitOfWork = new Mock<IUnitOfWork>();
            var campaignRepo = new Mock<ICampaignRepository>();
            var categoryRepo = new Mock<ICategoryRepository>();
            var currentUser = new Mock<ICurrentUserService>();

            currentUser.Setup(x => x.UserId)
                .Returns(data.CreatorId);

            var campaign = data.CreateCampaign();

            campaignRepo.Setup(x => x.GetByIdAsync(campaign.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Campaign?)null);

            var handler = new UpdateCampaignHandler(unitOfWork.Object, campaignRepo.Object, data.TimeProvider, currentUser.Object, categoryRepo.Object);
            
            var updateCommand = data.CreateUpdateCampaignCommand(campaign.Id);

            var result = await handler.Handle(updateCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();

            result.Error.Should().Be(Errors.Campaign.NotFound);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenConcurrencyConflict_ShouldReturnConcurrencyConflict()
        {
            var data = new CampaignTestData();

            var unitOfWork = new Mock<IUnitOfWork>();
            var campaignRepo = new Mock<ICampaignRepository>();
            var categoryRepo = new Mock<ICategoryRepository>();
            var currentUser = new Mock<ICurrentUserService>();

            currentUser.Setup(x => x.UserId)
                .Returns(data.CreatorId);

            var campaign = data.CreateCampaign();

            var category = data.CreateCategory();

            campaignRepo.Setup(x => x.GetByIdAsync(campaign.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(campaign);

            unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConcurrencyException());

            var handler = new UpdateCampaignHandler(unitOfWork.Object, campaignRepo.Object, data.TimeProvider, currentUser.Object, categoryRepo.Object);

            var updateCommand = data.CreateUpdateCampaignCommand(campaign.Id);            

            categoryRepo.Setup(x => x.GetByIdAsync(updateCommand.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var result = await handler.Handle(updateCommand, CancellationToken.None);

            result.Error.Should().Be(Errors.Campaign.ConcurrencyConflict);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
