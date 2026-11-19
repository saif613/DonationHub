using DonationHub.Application.Features.Campaigns.DTOs.Response;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.Queries.GetCampaignById
{
    public class GetCampaignByIdValidator: AbstractValidator<GetCampaignByIdQuery>
    {
        public GetCampaignByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required");

            RuleFor(x => x.Language)
               .NotEmpty()
               .Must(x => x == "ar" || x == "en")
               .WithMessage("Language must be ar or en");
        }
    }
}
