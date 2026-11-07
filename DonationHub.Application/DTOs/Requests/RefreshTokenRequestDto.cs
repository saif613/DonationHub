using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Application.DTOs.Requests
{
    public record RefreshTokenRequestDto
    (
        [Required]
       string RefreshToken
    );
}
