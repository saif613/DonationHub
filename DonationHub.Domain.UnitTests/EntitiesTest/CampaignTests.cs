
using Donation.Tests.Common;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;
using FluentAssertions;

namespace Donation.Tests.EntitiesTest
{
    public class CampaignTests
    {
        private static readonly Guid CategoryId = Guid.NewGuid();
        private static readonly Guid CreatorId = Guid.NewGuid();

        private static readonly DateTime Now = TestDateTime.Now;
        private static readonly DateTime FutureDate = Now.AddDays(30);

        private static Money CreateMoney(decimal amount, string currency)
        {
            var result = Money.Create(amount, currency);

            result.IsSuccess.Should().BeTrue();

            return result.Value;
        }

        private static Campaign CreateCampaign()
        {
            var result = Campaign.Create(
                "حملة علاج المرضى",
                "Patients Treatment",
                "وصف الحملة",
                "Campaign Description",
                CreateMoney(10000m, "EGP"),
                FutureDate,
                CategoryId,
                Now,
                CreatorId);

            result.IsSuccess.Should().BeTrue();

            return result.Value;
        }

        private static void ShouldSucceed(Result result)
        {
            result.IsSuccess.Should().BeTrue();
        }

        // Create Tests

        [Fact]
        public void Create_WithValidData_ShouldCreateCampaign()
        {
            var titleAr = "حملة علاج المرضى";
            var titleEn = "Patients Treatment Campaign";
            var descriptionAr = "وصف الحملة باللغة العربية";
            var descriptionEn = "Campaign description in English";

            var result = Campaign.Create(
                titleAr,
                titleEn,
                descriptionAr,
                descriptionEn,
                CreateMoney(10000m, "EGP"),
                FutureDate,
                CategoryId,
                Now,
                CreatorId);

            result.IsSuccess.Should().BeTrue();

            var campaign = result.Value;

            campaign.TitleAr.Should().Be(titleAr);
            campaign.TitleEn.Should().Be(titleEn);
            campaign.DescriptionAr.Should().Be(descriptionAr);
            campaign.DescriptionEn.Should().Be(descriptionEn);

            campaign.GoalAmount.Should().Be(CreateMoney(10000m, "EGP"));
            campaign.RaisedAmount.Should().Be(0);

            campaign.EndDate.Should().Be(FutureDate);
            campaign.CategoryId.Should().Be(CategoryId);

            campaign.Status.Should().Be(CampaignStatus.Active);

            campaign.CreatedAt.Should().Be(Now);
            campaign.CreatedBy.Should().Be(CreatorId);
        }

        [Theory]
        [InlineData("", "English Title", "Arabic Description", "English Description")]
        public void Create_WithInvalidArabicTitle_ShouldReturnFailure(
            string titleAr,
            string titleEn,
            string descriptionAr,
            string descriptionEn)
        {
            var result = Campaign.Create(
                titleAr,
                titleEn,
                descriptionAr,
                descriptionEn,
                CreateMoney(10000m, "EGP"),
                FutureDate,
                CategoryId,
                Now,
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.TitleArRequired);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_WithInvalidEnglishTitle_ShouldReturnFailure(string titleEn)
        {
            var result = Campaign.Create(
                "Arabic Title",
                titleEn,
                "Arabic Description",
                "English Description",
                CreateMoney(10000m, "EGP"),
                FutureDate,
                CategoryId,
                Now,
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.TitleEnRequired);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_WithInvalidArabicDescription_ShouldReturnFailure(string descriptionAr)
        {
            var result = Campaign.Create(
                "Arabic Title",
                "English Title",
                descriptionAr,
                "English Description",
                CreateMoney(10000m, "EGP"),
                FutureDate,
                CategoryId,
                Now,
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.DescriptionArRequired);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_WithInvalidEnglishDescription_ShouldReturnFailure(string descriptionEn)
        {
            var result = Campaign.Create(
                "Arabic Title",
                "English Title",
                "Arabic Description",
                descriptionEn,
                CreateMoney(10000m, "EGP"),
                FutureDate,
                CategoryId,
                Now,
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.DescriptionEnRequired);
        }

        [Fact]
        public void Create_WithNullGoalAmount_ShouldReturnFailure()
        {
            var result = Campaign.Create(
                "عنوان عربي",
                "English Title",
                "وصف عربي",
                "English Description",
                null!,
                FutureDate,
                CategoryId,
                Now,
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.GoalAmountRequired);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Create_WithInvalidEndDate_ShouldReturnFailure(int days)
        {
            var endDate = Now.AddDays(days);

            var result = Campaign.Create(
                "عنوان عربي",
                "English Title",
                "وصف عربي",
                "English Description",
                CreateMoney(10000m, "EGP"),
                endDate,
                CategoryId,
                Now,
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.EndDateInPast);
        }

        [Fact]
        public void Create_WithEmptyCategoryId_ShouldReturnFailure()
        {
            var result = Campaign.Create(
                "عنوان عربي",
                "English Title",
                "وصف عربي",
                "English Description",
                CreateMoney(10000m, "EGP"),
                FutureDate,
                Guid.Empty,
                Now,
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.CategoryIdRequired);
        }

        [Fact]
        public void Create_WithNullCreator_ShouldReturnFailure()
        {
            var result = Campaign.Create(
                "عنوان عربي",
                "English Title",
                "وصف عربي",
                "English Description",
                CreateMoney(10000m, "EGP"),
                FutureDate,
                CategoryId,
                Now,
                null);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.CreatorIdRequired);
        }

        [Fact]
        public void Create_WithEmptyCreator_ShouldReturnFailure()
        {
            var result = Campaign.Create(
                "عنوان عربي",
                "English Title",
                "وصف عربي",
                "English Description",
                CreateMoney(10000m, "EGP"),
                FutureDate,
                CategoryId,
                Now,
                Guid.Empty);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.CreatorIdRequired);
        }

        // ChangeGoal Tests

        [Fact]
        public void ChangeGoal_WithValidData_ShouldUpdateGoalAmount()
        {
            var campaign = CreateCampaign();
            var newGoal = CreateMoney(20000m, "EGP");

            var updatedAt = Now.AddDays(1);
            var updatedBy = Guid.NewGuid();

            var result = campaign.ChangeGoal(newGoal, updatedAt, updatedBy);

            result.IsSuccess.Should().BeTrue();

            campaign.GoalAmount.Should().Be(newGoal);
            campaign.UpdatedAt.Should().Be(updatedAt);
            campaign.UpdatedBy.Should().Be(updatedBy);
        }

        [Fact]
        public void ChangeGoal_WithNullGoalAmount_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            var result = campaign.ChangeGoal(null!, Now, CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.GoalAmountRequired);
        }

        [Fact]
        public void ChangeGoal_WithCompletedCampaign_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            ShouldSucceed(campaign.RegisterSuccessfulDonation(
                CreateMoney(10000m, "EGP"),
                Now.AddDays(1),
                CreatorId));

            var result = campaign.ChangeGoal(
                CreateMoney(20000m, "EGP"),
                Now.AddDays(2),
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.NotActive);
        }

        [Fact]
        public void ChangeGoal_WithDifferentCurrency_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            var result = campaign.ChangeGoal(
                CreateMoney(20000m, "USD"),
                Now.AddDays(1),
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.CurrencyMismatch);
        }

        [Fact]
        public void ChangeGoal_WithAmountLessThanRaisedAmount_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            ShouldSucceed(campaign.RegisterSuccessfulDonation(
                CreateMoney(5000m, "EGP"),
                Now.AddDays(1),
                CreatorId));

            var result = campaign.ChangeGoal(
                CreateMoney(4000m, "EGP"),
                Now.AddDays(2),
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.GoalBelowRaisedAmount);
        }

        [Fact]
        public void ChangeGoal_WithExpiredCampaign_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            ShouldSucceed(campaign.Expire(FutureDate, CreatorId));

            var result = campaign.ChangeGoal(
                CreateMoney(20000m, "EGP"),
                FutureDate.AddDays(1),
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.NotActive);
        }

        // ChangeEndDate Tests

        [Fact]
        public void ChangeEndDate_WithValidDate_ShouldUpdateEndDate()
        {
            var campaign = CreateCampaign();

            var newEndDate = Now.AddDays(60);
            var updatedAt = Now.AddDays(1);
            var updatedBy = Guid.NewGuid();

            var result = campaign.ChangeEndDate(newEndDate, updatedAt, updatedBy);

            result.IsSuccess.Should().BeTrue();

            campaign.EndDate.Should().Be(newEndDate);
            campaign.UpdatedAt.Should().Be(updatedAt);
            campaign.UpdatedBy.Should().Be(updatedBy);
        }

        [Fact]
        public void ChangeEndDate_WithCompletedCampaign_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            ShouldSucceed(campaign.RegisterSuccessfulDonation(
                CreateMoney(10000m, "EGP"),
                Now.AddDays(1),
                CreatorId));

            var result = campaign.ChangeEndDate(
                Now.AddDays(60),
                Now.AddDays(2),
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.NotActive);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void ChangeEndDate_WithInvalidDate_ShouldReturnFailure(int days)
        {
            var campaign = CreateCampaign();
            var invalidDate = Now.AddDays(days);

            var result = campaign.ChangeEndDate(invalidDate, Now, CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.EndDateInPast);
        }

        // CanAcceptDonation Tests

        [Fact]
        public void CanAcceptDonation_WithActiveCampaign_ShouldReturnTrue()
        {
            var campaign = CreateCampaign();

            var result = campaign.CanAcceptDonation(Now);

            result.Should().BeTrue();
        }

        [Fact]
        public void CanAcceptDonation_WithCompletedCampaign_ShouldReturnFalse()
        {
            var campaign = CreateCampaign();

            ShouldSucceed(campaign.RegisterSuccessfulDonation(
                CreateMoney(10000m, "EGP"),
                Now.AddDays(1),
                CreatorId));

            var result = campaign.CanAcceptDonation(Now.AddDays(2));

            result.Should().BeFalse();
        }

        [Fact]
        public void CanAcceptDonation_WithExpiredCampaign_ShouldReturnFalse()
        {
            var campaign = CreateCampaign();

            ShouldSucceed(campaign.Expire(FutureDate, CreatorId));

            var result = campaign.CanAcceptDonation(FutureDate);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void CanAcceptDonation_WithReachedEndDate_ShouldReturnFalse(int days)
        {
            var campaign = CreateCampaign();
            var currentDate = FutureDate.AddDays(days);

            var result = campaign.CanAcceptDonation(currentDate);

            result.Should().BeFalse();
        }

        // UpdateDetails Tests

        [Fact]
        public void UpdateDetails_WithCompletedCampaign_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            ShouldSucceed(campaign.RegisterSuccessfulDonation(
                CreateMoney(10000m, "EGP"),
                Now.AddDays(1),
                CreatorId));

            var result = campaign.UpdateDetails(
                "عنوان جديد",
                "New Title",
                "وصف جديد",
                "New Description",
                CategoryId,
                Now.AddDays(2),
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.NotActive);
        }

        [Fact]
        public void UpdateDetails_WithExpiredCampaign_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            ShouldSucceed(campaign.Expire(FutureDate, CreatorId));

            var result = campaign.UpdateDetails(
                "عنوان جديد",
                "New Title",
                "وصف جديد",
                "New Description",
                CategoryId,
                FutureDate.AddDays(1),
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.NotActive);
        }

        // RegisterSuccessfulDonation Tests

        [Fact]
        public void RegisterSuccessfulDonation_WithDifferentCurrency_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            var result = campaign.RegisterSuccessfulDonation(
                CreateMoney(100m, "USD"),
                Now,
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.CurrencyMismatch);
        }

        [Fact]
        public void RegisterSuccessfulDonation_WithExpiredCampaign_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            ShouldSucceed(campaign.Expire(FutureDate, CreatorId));

            var result = campaign.RegisterSuccessfulDonation(
                CreateMoney(100m, "EGP"),
                FutureDate.AddDays(1),
                CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.CannotAcceptDonation);
        }

        [Fact]
        public void RegisterSuccessfulDonation_WhenGoalReached_ShouldSetCompleted()
        {
            var campaign = CreateCampaign();

            var result = campaign.RegisterSuccessfulDonation(
                CreateMoney(10000m, "EGP"),
                Now.AddDays(1),
                CreatorId);

            result.IsSuccess.Should().BeTrue();

            campaign.RaisedAmount.Should().Be(10000m);
            campaign.Status.Should().Be(CampaignStatus.Completed);
        }

        // Expire Tests

        [Fact]
        public void Expire_BeforeEndDate_ShouldReturnFailure()
        {
            var campaign = CreateCampaign();

            var result = campaign.Expire(Now.AddDays(1), CreatorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Campaign.EndDateNotReached);
        }
    }
}
