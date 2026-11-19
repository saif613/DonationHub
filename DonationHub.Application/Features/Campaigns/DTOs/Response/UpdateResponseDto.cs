using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Campaigns.DTOs.Response
{
    public record UpdateResponseDto(
        Guid Id, 
        string TitleAr, 
        string TitleEn, 
        string DescriptionAr,
        string DescriptionEn,
        decimal Amount,
        string Currency,
        decimal RaisedAmount,
        DateTime EndDate,
        Guid CategoryId,
        byte[] RowVersion);    
}
