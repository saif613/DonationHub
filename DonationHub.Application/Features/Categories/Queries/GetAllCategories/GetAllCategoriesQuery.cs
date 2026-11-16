using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQuery : IRequest<Result<IReadOnlyList<CategoryResponseDto>>>;
    
}
