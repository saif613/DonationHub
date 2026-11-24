using DonationHub.Application.Features.Campaigns.Commands.CampaignCancelled;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Application.UnitTests.Common.TestData;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using FluentAssertions;
using Moq;

namespace DonationHub.Application.UnitTests.Features.Campaigns.Commands.CampignCancelled
{
    public class CampaignCancelledHandlerTests
    {
        [Fact]
        public async Task Handle_WhenCampaignNotFound_ShouldReturnNotFound()
        {
            var data = new CampaignTestData();

            var unitOfWork = new Mock<IUnitOfWork>();
            var campaignRepo = new Mock<ICampaignRepository>();
            var currentUser = new Mock<ICurrentUserService>();

            currentUser.Setup(x => x.UserId)
                .Returns(data.CreatorId);

            var command = new CampaignCancelledCommand(Guid.NewGuid());

            campaignRepo.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Campaign?)null);

            var handler = new CampaignCancelledHandler(unitOfWork.Object, data.TimeProvider, currentUser.Object, campaignRepo.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.NotFound);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCampaignNotActive_ShouldReturnNotActive()
        {
            var data = new CampaignTestData();

            var unitOfWork = new Mock<IUnitOfWork>();
            var campaignRepo = new Mock<ICampaignRepository>();
            var currentUser = new Mock<ICurrentUserService>();

            currentUser.Setup(x => x.UserId)
                .Returns(data.CreatorId);

            var campaign = data.CreateCampaign();
            var command = new CampaignCancelledCommand(campaign.Id);

            campaignRepo.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(campaign);

            var handler = new CampaignCancelledHandler(unitOfWork.Object, data.TimeProvider, currentUser.Object, campaignRepo.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.NotActive);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCampaignIsActive_ShouldCancelCampaign()
        {
            var data = new CampaignTestData();

            var unitOfWork = new Mock<IUnitOfWork>();
            var campaignRepo = new Mock<ICampaignRepository>();
            var currentUser = new Mock<ICurrentUserService>();

            currentUser.Setup(x => x.UserId)
                .Returns(data.CreatorId);

            var campaign = data.CreateCampaign();
            var command = new CampaignCancelledCommand(campaign.Id);

            campaignRepo.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(campaign);

            campaignRepo.Setup(x => x.IsCampaignActive(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var handler = new CampaignCancelledHandler(unitOfWork.Object, data.TimeProvider, currentUser.Object, campaignRepo.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            campaign.Status.Should().Be(CampaignStatus.Cancelled);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
