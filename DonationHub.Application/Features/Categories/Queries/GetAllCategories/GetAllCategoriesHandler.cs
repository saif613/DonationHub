using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetAllCategories
{
    public sealed class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, Result<IReadOnlyList<CategoryResponseDto>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetAllCategoriesHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<IReadOnlyList<CategoryResponseDto>>> Handle(GetAllCategoriesQuery request, CancellationToken ct = default)
        {
            var categories = await _categoryRepository.GetAllAsync(ct);

            var response = categories.Select(category =>
            new CategoryResponseDto(
                category.Id,
                category.NameAr,
                category.NameEn,
                category.DescriptionAr,
                category.DescriptionEn
                )).ToList();

            return Result.Success<IReadOnlyList<CategoryResponseDto>>(response);
        }
    }
}
