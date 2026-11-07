using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Helper
{
    internal class HelperText
    {
        internal static void ValidateText(string titleAr, string titleEn, string descriptionAr, string descriptionEn)
        {
            if (string.IsNullOrWhiteSpace(titleAr))
                throw new ArgumentException("Arabic title is required.");

            if (string.IsNullOrWhiteSpace(titleEn))
                throw new ArgumentException("English title is required.");

            if (string.IsNullOrWhiteSpace(descriptionAr))
                throw new ArgumentException("Arabic description is required.");

            if (string.IsNullOrWhiteSpace(descriptionEn))
                throw new ArgumentException("English description is required.");
        }
    }
}
