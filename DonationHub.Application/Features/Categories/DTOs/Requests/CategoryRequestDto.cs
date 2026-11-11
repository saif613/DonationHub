using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Application.Features.Category.DTOs.Requests
{
    public record CategoryRequestDto(
        [Required]
        [StringLength(100, MinimumLength = 1)]
        string NameAr,
        [Required]
        [StringLength(100, MinimumLength = 1)]
        string NameEn,
        string DescriptionAr = null!,
        string DescriptionEn = null!
        );
}
