using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.DTOs.Responses
{
    public record PagedResponse<T>(

        List<T> Data,
        int PageNumber,
        int PageSize,
        int TotalRecords
        )
    {
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    }    
    
}
