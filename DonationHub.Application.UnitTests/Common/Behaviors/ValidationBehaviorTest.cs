using DonationHub.Application.Behaviors;
using DonationHub.Domain.Common.Results;
using FluentAssertions;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.UnitTests.Common.Behaviors
{
    public class ValidationBehaviorTest
    {
        private record TestRequest(string name);

        [Fact]
        public async Task Handle_WhenValidationFails_ShouldReturnValidationFailedAndNotInvokeHandler()
        {
            var validator = new InlineValidator<TestRequest>();

            validator.RuleFor(x => x.name)
                .NotEmpty();

            var behavior = new ValidationBehavior<TestRequest, Result<string>>(new[] { validator });

            var request = new TestRequest("");

            var handlerCalled = false;

            RequestHandlerDelegate <Result<string>> next = (CancellationToken _) =>
            {
                handlerCalled = true;

                return Task.FromResult<Result<string>>(Result.Success("Handler executed"));
            };

            var result = await behavior.Handle(request, next, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Validation.Failed);
            handlerCalled.Should().BeFalse();
        }
    }
}
