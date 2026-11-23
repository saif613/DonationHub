using DonationHub.Application.Features.Campaigns.Queries.GetCampaignById;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.UnitTests.Common.TestData;
using DonationHub.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.UnitTests.Features.Campaigns.Queries.GetCampaignById
{
    public class GetCampaignByIdHandlerTests
    {
        [Theory]
        [InlineData(5000, 50)]
        [InlineData(10000, 100)]
        [InlineData(15000, 100)]
        public async Task Handle_WhenCalculatingProgressPercent_ShouldReturnExpectedValue(decimal raisedAmount, decimal expectedProgress)
        {
            var data = new CampaignTestData();
            var campaign = data.CreateCampaign();

            var donationResult = campaign.RegisterSuccessfulDonation(
                Money.Create(raisedAmount, "EGP").Value,
                data.Now,
                data.CreatorId);

            donationResult.IsSuccess.Should().BeTrue();

            var campaignRepo = new Mock<ICampaignRepository>();

            campaignRepo.Setup(x => x.GetByIdAsync(campaign.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(campaign);

            var timeProvider = data.TimeProvider;

            var handler = new GetCampaignByIdHandler(campaignRepo.Object, timeProvider);

            var query = new GetCampaignByIdQuery(campaign.Id, "en");

            var result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            result.Value.ProgressPercent.Should().Be(expectedProgress);
        }

        [Fact]
        public async Task Handle_WhenTimeAdvances_ShouldReturnUpdatedDaysLeft()
        {
            var data = new CampaignTestData();
            var campaign = data.CreateCampaign();
           
            var campaignRepo = new Mock<ICampaignRepository>();

            campaignRepo.Setup(x => x.GetByIdAsync(campaign.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(campaign);

            var timeProvider = data.TimeProvider;

            var handler = new GetCampaignByIdHandler(campaignRepo.Object, timeProvider);

            var query = new GetCampaignByIdQuery(campaign.Id, "en");

            timeProvider.Advance(TimeSpan.FromDays(3));

            var result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            result.Value.DaysLeft.Should().Be(27);
        }

        [Fact]
        public async Task Handle_WhenCampaignHasEnded_ShouldReturnZeroDaysLeft()
        {
            var data = new CampaignTestData();
            var campaign = data.CreateCampaign();

            var campaignRepo = new Mock<ICampaignRepository>();

            campaignRepo.Setup(x => x.GetByIdAsync(campaign.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(campaign);

            var timeProvider = data.TimeProvider;

            var handler = new GetCampaignByIdHandler(campaignRepo.Object, timeProvider);

            var query = new GetCampaignByIdQuery(campaign.Id, "en");

            timeProvider.Advance(TimeSpan.FromDays(31));

            var result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();

            result.Value.DaysLeft.Should().Be(0);
        }
    }
}
