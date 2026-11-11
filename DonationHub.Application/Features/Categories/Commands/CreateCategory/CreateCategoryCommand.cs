using DonationHub.Application.Features.Category.DTOs.Requests;
using DonationHub.Application.Features.Category.DTOs.Responses;
using DonationHub.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string NameAr, string NameEn, string? DescriptionAr, string? DescriptionEn) : IRequest<Result<CategoryResponseDto>>;    
}
