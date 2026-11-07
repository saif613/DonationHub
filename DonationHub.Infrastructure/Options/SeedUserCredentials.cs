using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Infrastructure.Options
{
    public class SeedUserCredentials
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [StringLength(150, MinimumLength = 2)]        
        public string FullName { get; set; } = string.Empty;
    }
}
