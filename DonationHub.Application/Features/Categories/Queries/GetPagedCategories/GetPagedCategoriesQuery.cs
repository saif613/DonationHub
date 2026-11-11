using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Application.Features.DTOs.Responses;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetPagedCategories
{
    public record GetPagedCategoriesQuery(int PageNumber, int PageSize) : IRequest<Result<PagedResponse<CategoryResponseDto>>>;
    
}
