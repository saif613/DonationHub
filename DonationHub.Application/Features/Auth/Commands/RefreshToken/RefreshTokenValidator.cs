using FluentValidation;

namespace DonationHub.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenValidator: AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
           .NotEmpty()
           .WithMessage("RefreshToken is required");
    }
}
