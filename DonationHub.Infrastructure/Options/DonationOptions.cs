using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Infrastructure.Options
{
    public class DonationOptions
    {
        public const string SectionName = "DonationOptions";
        [Range(0.01, double.MaxValue)]
        public decimal MinimumDonationAmount { get; set; } = 1.00m;
        [Range(0.01, double.MaxValue)]
        public decimal MaximumDonationAmount { get; set; } = 10000.00m;
        [Range(1, int.MaxValue)]
        public int PendingTimeoutMinutes { get; set; } = 30;
        [Required]
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Default currency must be a 3-letter ISO currency code.")]      
        public string DefaultCurrency { get; set; } = "USD";
    }
}
