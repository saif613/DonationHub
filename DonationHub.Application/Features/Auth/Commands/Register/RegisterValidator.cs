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
            .WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.");
    }
}
