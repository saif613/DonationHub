using DonationHub.Domain.Common;
using DonationHub.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities;

public class Category : SoftDeletableEntity
{
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public string? DescriptionAr { get; private set; } 
    public string? DescriptionEn { get; private set; } 

    private Category() { }

    public static Category Create(string nameAr, string nameEn, string descriptionAr, string descriptionEn, DateTime utcNow, Guid? createdBy)
    {

        ValidateNames(nameAr, nameEn);

        return new Category
        {
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim(),
            DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim()    ,
            CreatedAt = utcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string nameAr, string nameEn, string descriptionAr, string descriptionEn, DateTime utcNow, Guid? userID)
    {
        ValidateNames(nameAr, nameEn);

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim();
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();

        MarkAsUpdated(utcNow, userID);
    }  
    
    private static void ValidateNames(string nameAr, string
        nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new ArgumentException("Arabic Name is Required.");

        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("English Name is Required");

        if (nameAr.Trim().Length > 100)
            throw new ArgumentException("Arabic name cannot exceed 100 characters");

        if (nameEn.Trim().Length > 100)
            throw new ArgumentException("English name cannot exceed 100 characters");
    }
}
