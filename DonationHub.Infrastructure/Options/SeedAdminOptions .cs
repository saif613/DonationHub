using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Infrastructure.Options
{
    public class SeedAdminOptions
    {
        public const string SectionName = "SeedAdmin";
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = null!;
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; } = null!;
        [Required]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 150 characters.")]
        public string FullName { get; set; } = null!;
        public string PreferredLanguage { get; set; } = "en";
    }
}
