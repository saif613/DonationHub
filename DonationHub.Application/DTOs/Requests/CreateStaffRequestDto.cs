using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Application.DTOs.Requests
{
    public record CreateStaffRequestDto(
        [Required]
        [EmailAddress(ErrorMessage = "InvalidEmailFormat")]
        string Email,
        [Required]
        [StringLength(150, MinimumLength = 2)]
        string FullName,
        [Required]
        string Role,
        string PreferredLanguage = "en"
    );

}
