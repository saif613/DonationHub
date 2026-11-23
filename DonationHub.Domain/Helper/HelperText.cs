using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Helper
{
    internal static class HelperText
    {
        internal static Result ValidateText(string titleAr, string titleEn, string descriptionAr, string descriptionEn)
        {
            if (string.IsNullOrWhiteSpace(titleAr))
                return Errors.Campaign.TitleArRequired;

            if (string.IsNullOrWhiteSpace(titleEn))
                return Errors.Campaign.TitleEnRequired;

            if (string.IsNullOrWhiteSpace(descriptionAr))
                return Errors.Campaign.DescriptionArRequired;

            if (string.IsNullOrWhiteSpace(descriptionEn))
                return Errors.Campaign.DescriptionEnRequired;

            return Result.Success();
        }
    }
}
