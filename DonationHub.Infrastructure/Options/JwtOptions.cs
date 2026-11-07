using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Infrastructure.Options
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        [Required]
        [MinLength(32, ErrorMessage = "Secret key must be at least 32")]
        public string SecretKey { get; set; } = string.Empty;
        [Required]
        public string Issuer { get; set; } = string.Empty;
        [Required]
        public string Audience { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Expiry minutes must be greater than 0")]
        public int ExpiryMinutes { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Refresh token expiry days must be greater than 0")]
        public int RefreshTokenExpiryDays { get; set; }        

    }
}
