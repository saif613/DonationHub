using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Application.Features.DTOs.Requests
{
    public record ChangePasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; init; } = string.Empty;

        [Required, MinLength(8)]
        public string NewPassword { get; init; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; init; } = string.Empty;
    }          
  
}
