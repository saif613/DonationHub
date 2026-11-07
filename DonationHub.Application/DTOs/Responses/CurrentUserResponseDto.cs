using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.DTOs.Responses
{
    public record CurrentUserResponseDto(
        Guid Id,
        string Email,
        string FullName,
        string PreferredLanguage,
        IReadOnlyList<string> Roles
    );
}
