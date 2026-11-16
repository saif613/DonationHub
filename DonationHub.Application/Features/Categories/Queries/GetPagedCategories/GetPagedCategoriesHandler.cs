using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Application.Features.DTOs.Responses;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetPagedCategories
{
    public sealed class GetPagedCategoriesHandler: IRequestHandler<GetPagedCategoriesQuery, Result<PagedResponse<CategoryResponseDto>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetPagedCategoriesHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<PagedResponse<CategoryResponseDto>>> Handle(GetPagedCategoriesQuery request, CancellationToken ct = default)
        {
            var (categories, totalCount) = await _categoryRepository.GetPagedAsync(request.PageNumber, request.PageSize, ct);

            var data = categories.Select(category => new CategoryResponseDto(category.Id, category.NameAr, category.NameEn,
           category.DescriptionAr,
           category.DescriptionEn))
          .ToList();

            var response =  new PagedResponse<CategoryResponseDto>(data, request.PageNumber, request.PageSize, totalCount);

            return Result.Success(response);
        }
    }
}
