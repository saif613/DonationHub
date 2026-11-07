using Donation.Tests.Common;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donation.Tests.EntitiesTest
{
    public class CampaignTests
    {
        private Campaign CreateCampaign()
        {
            return Campaign.Create("حملة علاج المرضى", "Patients Treatment", "وصف الحملة", "Campaign Description", GoalAmount, FutureDate, CategoryId, Now,   CreatorId);
        }
        private static readonly Guid CategoryId = Guid.NewGuid();
        private static readonly Guid CreatorId = Guid.NewGuid();
        private static readonly Money GoalAmount = Money.Create(10000m, "EGP");
        private static readonly DateTime Now = TestDateTime.Now;
        private static readonly DateTime FutureDate = Now.AddDays(30);

        [Fact]
        public void Create_WithValidData_ShouldCreateCampaign()
        {
            var titleAr = "حملة علاج المرضى";
            var titleEn = "Patients Treatment Campaign";
            var descriptionAr = "وصف الحملة باللغة العربية";
            var descriptionEn = "Campaign description in English";

            var campaign = Campaign.Create(titleAr, titleEn, descriptionAr, descriptionEn, GoalAmount, FutureDate, CategoryId, Now, CreatorId);

            campaign.Should().NotBeNull();

            campaign.TitleAr.Should().Be(titleAr);
            campaign.TitleEn.Should().Be(titleEn);
            campaign.DescriptionAr.Should().Be(descriptionAr);
            campaign.DescriptionEn.Should().Be(descriptionEn);

            campaign.GoalAmount.Should().Be(GoalAmount);
            campaign.RaisedAmount.Should().Be(0);

            campaign.EndDate.Should().Be(FutureDate);
            campaign.CategoryId.Should().Be(CategoryId);

            campaign.Status.Should().Be(CampaignStatus.Active);

            campaign.CreatedAt.Should().Be(Now);
            campaign.CreatedBy.Should().Be(CreatorId);
        }

        [Theory]
        [InlineData("", "English Title", "Arabic Description", "English Description", "Arabic title is required.")]

        [InlineData("Arabic Title", " ", "Arabic Description", "English Description", "English title is required.")]

        [InlineData("Arabic Title", "English Title", "", "English Description", "Arabic description is required.")]

        [InlineData("Arabic Title", "English Title", "Arabic Description", " ", "English description is required.")]
        public void Create_WithInvalidText_ShouldThrowArgumentException(string titleAr, string titleEn, string descriptionAr, string descriptionEn, string expectedMessage)
        {
            var act = () => Campaign.Create(titleAr, titleEn, descriptionAr, descriptionEn, GoalAmount, FutureDate, CategoryId, Now, CreatorId);

            act.Should().Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void Create_WithNullGoalAmount_ShouldThrowArgumentNullException()
        {
            var act = () => Campaign.Create("عنوان عربي", "English Title", "وصف عربي", "English Description", null!, FutureDate, CategoryId, Now, CreatorId);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("goalAmount");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Create_WithInvalidEndDate_ShouldThrowArgumentException(int days)
        {
            var endDate = Now.AddDays(days);

            var act = () => Campaign.Create("عنوان عربي", "English Title", "وصف عربي", "English Description", GoalAmount, endDate, CategoryId, Now, CreatorId);

            act.Should().Throw<ArgumentException>()
                .WithMessage("End date must be in the future.");
        }

        [Fact]
        public void Create_WithEmptyCategoryId_ShouldThrowArgumentException()
        {

            var act = () => Campaign.Create("عنوان عربي", "English Title", "وصف عربي", "English Description", GoalAmount, FutureDate, Guid.Empty, Now, CreatorId);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Category id is required.");

        }

        [Fact]
        public void Create_WithNullCreator_ShouldThrowArgumentException()
        {

            var act = () => Campaign.Create("عنوان عربي", "English Title", "وصف عربي", "English Description", GoalAmount, FutureDate, CategoryId, Now, null);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Creator id is required.");

        }


        [Fact]
        public void Create_WithEmptyCreator_ShouldThrowArgumentException()
        {

            var act = () => Campaign.Create("عنوان عربي", "English Title", "وصف عربي", "English Description", GoalAmount, FutureDate, CategoryId, Now, Guid.Empty);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Creator id is required.");

        }

        [Fact]
        public void ChangeGoal_WithValidDate_ShouldUpdateGoalAmount()
        {
            var campaign = CreateCampaign();
            var newGoal = Money.Create(2000m, "EGP");

            var updatedAt = Now.AddDays(1);
            var updatedBy = Guid.NewGuid();

            campaign.ChangeGoal(newGoal, updatedAt, updatedBy);

            campaign.GoalAmount.Should().Be(newGoal);
            campaign.UpdatedAt.Should().Be(updatedAt);
            campaign.UpdatedBy.Should().Be(updatedBy);
        }

        [Fact]
        public void ChangeGoal_WithNullGoalAmount_ShouldThrowArgumentNullException()
        {
            var campaign = CreateCampaign();

            var act = () => campaign.ChangeGoal(null!, Now, CreatorId);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("newGoalAmount");
        }

        [Fact]
        public void ChangeGoal_WithCompletedCampaign_ShouldThrowInvalidOperationException()
        {
            var campaign = CreateCampaign();

            campaign.RegisterSuccessfulDonation(Money.Create(10000m, "EGP"), Now.AddDays(1), CreatorId);

            var act = () => campaign.ChangeGoal(Money.Create(20000m, "EGP"), Now.AddDays(2), CreatorId);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Only active campaigns can change their goal.");
        }

        [Fact]
        public void ChangeGoal_WithDifferentCurrency_ShouldThrowInvalidOperationException()
        {
            var campaign = CreateCampaign();
            var newGoal = Money.Create(20000m, "USD");


            var act = () => campaign.ChangeGoal(newGoal, Now.AddDays(1), CreatorId);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Campaign currency cannot be changed.");
        }

        [Fact]
        public void ChangeGoal_WithAmountLessThanRaisedAmount_ShouldThrowArgumentOutOfRangeException()
        {
            var campaign = CreateCampaign();

            campaign.RegisterSuccessfulDonation(Money.Create(5000m, "EGP"), Now.AddDays(1), CreatorId);

            var newGoal = Money.Create(4000m, "EGP");

            var act = () => campaign.ChangeGoal(newGoal, Now.AddDays(2), CreatorId);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("newGoalAmount");
        }

        [Fact]
        public void ChangeEndDate_WithValidDate_ShouldUpdateEndDate()
        {
            var campaign = CreateCampaign();

            var newEndDate = Now.AddDays(60);
            var updatedAt = Now.AddDays(1);
            var updatedBy = Guid.NewGuid();

            campaign.ChangeEndDate(newEndDate, updatedAt, updatedBy);

            campaign.EndDate.Should().Be(newEndDate);
            campaign.UpdatedAt.Should().Be(updatedAt);
            campaign.UpdatedBy.Should().Be(updatedBy);
        }
        [Fact]
        public void ChangeEndDate_WithCompletedCampaign_ShouldThrowInvalidOperationException()
        {
            var campaign = CreateCampaign();

            campaign.RegisterSuccessfulDonation(Money.Create(10000m, "EGP"), Now.AddDays(1), CreatorId);

            var newEndDate = Now.AddDays(60);

            var act = () => campaign.ChangeEndDate(newEndDate, Now.AddDays(2), CreatorId);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Only active campaigns can change their end date.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void ChangeEndDate_WithInvalidDate_ShouldThrowArgumentException(int days)
        {
            var campaign = CreateCampaign();
            var invalidDate = Now.AddDays(days);

            var act = () => campaign.ChangeEndDate(invalidDate, Now, CreatorId);

            act.Should().Throw<ArgumentException>()
                .WithMessage("End date must be in the future.");
        }

        [Fact]
        public void CanAcceptDonation_WithActiveCampaign_ShouldReturnTrue()
        {
            var campaign = CreateCampaign();

            var result = campaign.CanAcceptDonation(Now);

            result.Should().BeTrue();
        }

        [Fact]
        public void CanAcceptDonation_WithCampaignCampaign_ShouldReturnFalse()
        {
            var campaign = CreateCampaign();
            campaign.RegisterSuccessfulDonation(Money.Create(10000m, "EGP"), Now.AddDays(1), CreatorId);

            var result = campaign.CanAcceptDonation(Now.AddDays(2));

            result.Should().BeFalse();
        }

        [Fact]
        public void CanAcceptDonation_WithExpiredCampaign_ShouldReturnFalse()
        {
            var campaign = CreateCampaign();

            campaign.Expire(FutureDate, CreatorId);
            var result = campaign.CanAcceptDonation(FutureDate);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void CanAcceptDonation_WithReachedEndDat_ShouldReturnFalse(int days)
        {
            var campaign = CreateCampaign();
            var currentDate = FutureDate.AddDays(days);

            
            var result = campaign.CanAcceptDonation(currentDate);

            result.Should().BeFalse();
        }
    }
}
