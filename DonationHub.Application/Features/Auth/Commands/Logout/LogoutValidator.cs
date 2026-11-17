using FluentValidation;

namespace DonationHub.Application.Features.Auth.Commands.Logout;

public sealed class LogoutValidator: AbstractValidator<LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("RefreshToken is required");
    }
}
