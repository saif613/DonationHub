using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetPagedCategories
{
    public class GetPagedCategoriesValidator: AbstractValidator<GetPagedCategoriesQuery>
    {
        public GetPagedCategoriesValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);
        }
    }
}
