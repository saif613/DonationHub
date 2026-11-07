using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Application.DTOs.Requests
{
    public record RegisterRequestDto(
        [Required]
        [EmailAddress]
        string Email,
        [Required]
        [StringLength(150, MinimumLength = 2)]
        string FullName,
        [Required]
        string Password,
        string PreferredLanguage = "en"
    );
}
