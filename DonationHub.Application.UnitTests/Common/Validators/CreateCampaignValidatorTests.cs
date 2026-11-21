using DonationHub.Application.Features.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.UnitTests.Common.TestData;
using FluentAssertions;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.UnitTests.Common.Validators
{
    public class CreateCampaignValidatorTests
    {
        private readonly CreateCampaignValidator _validator;

        public CreateCampaignValidatorTests()
        {
            _validator = new CreateCampaignValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_WhenTitleArIsEmpty_ShouldHaveValidationError(string? titleAr)
        {
            var data = new CampaignTestData();

            var command = data.CreateCampaignCommand() with
            {
                TitleAr = titleAr!
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.TitleAr);
        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_WhenTitleEnIsEmpty_ShouldHaveValidationError(string? titleEn)
        {
            var data = new CampaignTestData();

            var command = data.CreateCampaignCommand() with
            {
                TitleEn = titleEn!
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.TitleEn);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Validate_WhenGoalAmountIsInvalid_ShouldHaveValidationError(decimal amount)
        {
            var data = new CampaignTestData();

            var command = data.CreateCampaignCommand() with
            {
                Amount = amount!
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Amount);
        }

        [Fact]
        public void Validate_WhenCategoryIdIsEmpty_ShouldHaveValidationError()
        {
            var data = new CampaignTestData();

            var command = data.CreateCampaignCommand() with
            {
                CategoryId = Guid.Empty
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }
    }
}
