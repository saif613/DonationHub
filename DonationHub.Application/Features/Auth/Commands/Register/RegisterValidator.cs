using FluentValidation;

namespace DonationHub.Application.Features.Auth.Commands.Register;

public sealed class RegisterValidator: AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("FullName is required")
            .MaximumLength(150)
            .WithMessage("FullName exceed 150 characters")
            .MinimumLength(2)
            .WithMessage("FullName too small");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");
    }
}
