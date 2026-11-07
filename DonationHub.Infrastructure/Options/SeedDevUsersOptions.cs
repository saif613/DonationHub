using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Infrastructure.Options
{
    public class SeedDevUsersOptions
    {
        public const string SectionName = "SeedDevUsers";

        [Required]
        public SeedUserCredentials CampaignManager { get; set; } = new() ;
        [Required]
        public SeedUserCredentials Donor { get; set; } = new();
    }
    
}
