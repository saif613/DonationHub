using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.DTOs.Responses
{
    public record TokenResponseDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; init; }
        public DateTime RefreshTokenExpiresAt { get; init; }
        public bool Success { get; init;  }
        public string Message { get; init; } = string.Empty;
    }
    
}
