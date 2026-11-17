using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Users.DTOs
{
    public record UserResponseDto(
         string Email,
         string FullName,
         string PreferredLanguage,
         bool IsActive
        );
}
