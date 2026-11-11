using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.UpdateCategory
{
    public sealed class UpdateCategoryValidator:AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.NameAr)
              .NotEmpty()
              .WithMessage("Arabic category name is required")
              .MaximumLength(100);


            RuleFor(x => x.NameEn)
                .NotEmpty()
                .WithMessage("English category name is required")
                .MaximumLength(100);
        }
    }
}
