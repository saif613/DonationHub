using FluentValidation;

namespace DonationHub.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordValidator: AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("new password is required")
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("Password confirmation is required")
            .Equal(x => x.NewPassword)
            .WithMessage("New password and confirmation do not match");
    }
}
