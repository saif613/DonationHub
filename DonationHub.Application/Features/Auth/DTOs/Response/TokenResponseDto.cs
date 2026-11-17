using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Auth.DTOs.Response
{
    public record TokenResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt
);
}
