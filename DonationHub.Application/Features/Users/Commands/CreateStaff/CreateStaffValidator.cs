using FluentValidation;

namespace DonationHub.Application.Features.Users.Commands.CreateStaff;

public sealed class CreateStaffValidator: AbstractValidator<CreateStaffCommand>
{
    public CreateStaffValidator()
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

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required");
    }
}
