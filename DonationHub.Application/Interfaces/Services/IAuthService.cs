using DonationHub.Application.DTOs.Requests;
using DonationHub.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<TokenResponseDto> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken ct = default);
        Task RegisterAsync(RegisterRequestDto registerRequestDto, CancellationToken ct = default);
        Task<TokenResponseDto> RefreshAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken ct = default);
        Task LogoutAsync(LogoutRequestDto logoutRequestDto, CancellationToken ct = default);
        Task<CurrentUserResponseDto> GetCurrentUserAsync(CancellationToken ct = default);
        Task ChangePasswordAsync(ChangePasswordRequestDto request, CancellationToken ct = default);
    }
}
