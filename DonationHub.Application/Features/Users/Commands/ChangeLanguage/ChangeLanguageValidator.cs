using FluentValidation;

namespace DonationHub.Application.Features.Users.Commands.ChangeLanguage;

public sealed class ChangeLanguageValidator: AbstractValidator<ChangeLanguageCommand>
{
    public ChangeLanguageValidator()
    {
        RuleFor(x => x.Language)
            .NotEmpty()
            .WithMessage("Language is required")
            .Must(language => language is "ar" or "en")
            .WithMessage("Unsupported language");
    }
}
