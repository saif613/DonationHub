using DonationHub.Application.Exceptions;
using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetCategoryById
{
    public sealed class GetCategoryByIdHandler: IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponseDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryByIdHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<CategoryResponseDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct = default)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, ct);

            if (category is null)
                return Errors.Category.NotFound;

            var response = new CategoryResponseDto(category.Id, category.NameAr, category.NameEn, category.DescriptionAr, category.DescriptionEn);

            return Result.Success(response);
        }
    }
}
