using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Application.Features.Category.DTOs.Responses
{
    public record CategoryResponseDto(
        Guid id,
        string NameAr,
        string NameEn,
        string? DescriptionAr,
        string? DescriptionEn
        );
   
}
