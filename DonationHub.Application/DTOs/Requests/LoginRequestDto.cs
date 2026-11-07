using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Application.DTOs.Requests
{
    public record LoginRequestDto(
        [Required]
        [EmailAddress]
        string Email,
        [Required]
        string Password
    );
}
