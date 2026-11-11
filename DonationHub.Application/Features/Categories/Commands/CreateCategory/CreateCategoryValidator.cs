using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryValidator: AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.NameAr)
                .NotEmpty()
                .WithMessage("Arabic category name is required")
                .MaximumLength(100)
                .WithMessage("Arabic category name cannot exceed 100 characters");

            RuleFor(x => x.NameEn)
                .NotEmpty()
                .WithMessage("English category name is required")
                .MaximumLength(100)
                .WithMessage("English category name cannot exceed 100 characters");
        }
    }
}
