using DonationHub.Application.Features.Auth.Commands.Register;
using DonationHub.Application.Features.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.UnitTests.Common.TestData;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.UnitTests.Common.Validators
{   
    public class RegisterCommandValidatorTest
    {
        public RegisterCommand CreateRegisterCommand()
        {
            return new RegisterCommand(
                Email: "test@example.com",
                FullName: "Test User",
                Password: "Abcdefgh1");
        }

        private readonly RegisterValidator _validator;

        public RegisterCommandValidatorTest()
        {
            _validator = new RegisterValidator();
        }

        [Theory]
        [InlineData("Abc123")]      
        [InlineData("abcdefgh1")]   
        [InlineData("ABCDEFGH1")]   
        [InlineData("Abcdefgh")]
        public void Validate_WhenPasswordDoesNotMeetPolicy_ShouldHaveValidationError(string password)
        {
            var data = new RegisterCommandValidatorTest();

            var command = CreateRegisterCommand() with
            {
                Password = password!
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}
