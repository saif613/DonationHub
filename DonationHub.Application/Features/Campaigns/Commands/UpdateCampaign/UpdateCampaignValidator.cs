using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign
{
    public class UpdateCampaignValidator: AbstractValidator<UpdateCampaignCommand>
    {
        public UpdateCampaignValidator()
        {
            RuleFor(x => x.TitleAr)
             .NotEmpty()
             .WithMessage("Arabic title is required");

            RuleFor(x => x.TitleEn)
                .NotEmpty()
                .WithMessage("English title is required.");

            RuleFor(x => x.DescriptionAr)
                .NotEmpty()
                .WithMessage("Arabic description is required");

            RuleFor(x => x.DescriptionEn)
                .NotEmpty()
                .WithMessage("English description is required");

            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Category is required");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero");

            RuleFor(x => x.Currency)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(currency =>
                    currency.Trim().ToUpperInvariant() is "EGP" or "USD")
                .WithMessage("Currency must be EGP or USD");

            RuleFor(x => x.RowVersion)
               .NotEmpty()
               .WithMessage("RowVersion is required.");
        }
    }
}
