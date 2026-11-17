using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Auth.DTOs.Response
{
    public record CurrentUserResponseDto(
        Guid Id,
        string Email,
        string FullName,
        string PreferredLanguage,
        IReadOnlyList<string> Roles
    );
}
