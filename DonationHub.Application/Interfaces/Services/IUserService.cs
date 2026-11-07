using DonationHub.Application.DTOs.Requests;
using DonationHub.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> ChangeUserLanguageAsync(Guid userId, string language, CancellationToken ct = default);
        Task<UserResponseDto> DeactivateUserAsync(Guid userId, CancellationToken ct = default);
        Task<UserResponseDto> ActivateUserAsync(Guid userId, CancellationToken ct = default);
        Task<CreateStaffResponseDto> CreateStaffAsync(CreateStaffRequestDto request, CancellationToken ct = default);
        Task<UserResponseDto> UpdateProfileAsync(UpdateProfileRequestDto request, CancellationToken ct = default);
    }
}
