using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.DTOs.Responses
{
    public record UserResponseDto(
         string Email,
         string FullName,
         string PreferredLanguage,
         bool IsActive
        );
}
