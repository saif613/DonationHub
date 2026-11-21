using DonationHub.Application.Features.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Application.UnitTests.Common.TestData;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;


namespace DonationHub.Application.UnitTests.Features.Campaigns.Commands.CreateCampaign
{
    public class CreateCampaignHandlerTests
    {
        [Fact]
        public async Task Handle_WhenCategoryNotFound_ShouldReturnCategoryNotFound()
        {
            var data = new CampaignTestData();

            var unitOfWork = new Mock<IUnitOfWork>();
            var campaignRepo = new Mock<ICampaignRepository>();
            var categoryRepo = new Mock<ICategoryRepository>();
            var currentUser = new Mock<ICurrentUserService>();

            currentUser.Setup(x => x.UserId)
                .Returns(data.CreatorId);

            categoryRepo.Setup(x => x.GetByIdAsync(data.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var handler = new CreateCampaignHandler(unitOfWork.Object, campaignRepo.Object, data.TimeProvider, currentUser.Object, categoryRepo.Object);

            var command = data.CreateCampaignCommand();

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();

            result.Error.Should().Be(Errors.Category.NotFound);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithSuccessCreation()
        {
            var data = new CampaignTestData();

            var unitOfWork = new Mock<IUnitOfWork>();
            var campaignRepo = new Mock<ICampaignRepository>();
            var categoryRepo = new Mock<ICategoryRepository>();
            var currentUser = new Mock<ICurrentUserService>();

            currentUser.Setup(x => x.UserId)
                .Returns(data.CreatorId);

            var category = data.CreateCategory();

            categoryRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            campaignRepo.Setup(x => x.ExistsByTitleArAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            campaignRepo.Setup(x => x.ExistsByTitleEnAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
               .ReturnsAsync(false);


            var handler = new CreateCampaignHandler(unitOfWork.Object, campaignRepo.Object, data.TimeProvider, currentUser.Object, categoryRepo.Object);

            var command = data.CreateCampaignCommand();

            var result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            result.Value.titleAr.Should().Be(command.TitleAr);
            result.Value.titleEn.Should().Be(command.TitleEn);

            campaignRepo.Verify(x => x.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()), Times.Once);

            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
