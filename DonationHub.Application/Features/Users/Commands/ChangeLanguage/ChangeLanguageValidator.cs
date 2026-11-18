using FluentValidation;

namespace DonationHub.Application.Features.Users.Commands.ChangeLanguage;

public sealed class ChangeLanguageValidator: AbstractValidator<ChangeLanguageCommand>
{
    public ChangeLanguageValidator()
    {
        RuleFor(x => x.Language)
            .NotEmpty()
            .WithMessage("Language is required")
            .Must(language => string.Equals(language, "ar", StringComparison.OrdinalIgnoreCase) || string.Equals(language, "en", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Language must be either 'ar' or 'en'");            
    }
}
