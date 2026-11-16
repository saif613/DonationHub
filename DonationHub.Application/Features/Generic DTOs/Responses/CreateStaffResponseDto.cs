using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.DTOs.Responses
{
    public record CreateStaffResponseDto(
        Guid UserId,
        string Email,
        string FullName,       
        string Role,
        string TemporaryPassword
    );
}
