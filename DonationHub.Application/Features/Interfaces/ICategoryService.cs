using DonationHub.Application.DTOs.Requests;
using DonationHub.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateAsync(CategoryRequestDto request, CancellationToken ct = default);
        Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default);
        Task<CategoryResponseDto> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<CategoryResponseDto> Update(CategoryRequestDto request, Guid id, CancellationToken ct = default);
        Task SoftDelete(Guid id, CancellationToken ct = default);
        Task<PagedResponse<CategoryResponseDto>> PagedAsync(int PageNumber, int PageSize, CancellationToken ct = default);
    }
}
