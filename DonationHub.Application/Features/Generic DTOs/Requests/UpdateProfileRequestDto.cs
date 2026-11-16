using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Application.Features.DTOs.Requests
{
    public record UpdateProfileRequestDto(
        [Required]
        [StringLength(100)]
        string FullName,

        [Required]
        [EmailAddress]
        string? Email
        );
    
}
