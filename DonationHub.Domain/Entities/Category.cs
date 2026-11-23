using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
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

    public static Result<Category> Create(string nameAr, string nameEn, string descriptionAr, string descriptionEn, DateTime utcNow, Guid? createdBy)
    {

        var validationResult = ValidateNames(nameAr, nameEn);

        if (validationResult.IsFailure)
            return validationResult.Error;

        var category = new Category
        {
            NameAr = nameAr.Trim(),
            NameEn = nameEn.Trim(),
            DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim(),
            DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim()    ,
            CreatedAt = utcNow,
            CreatedBy = createdBy
        };

        return Result.Success(category);
    }

    public Result Update(string nameAr, string nameEn, string descriptionAr, string descriptionEn, DateTime utcNow, Guid? userID)
    {
        var validationResult = ValidateNames(nameAr, nameEn);

        if (validationResult.IsFailure)
            return validationResult.Error;

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        DescriptionAr = string.IsNullOrWhiteSpace(descriptionAr) ? null : descriptionAr.Trim();
        DescriptionEn = string.IsNullOrWhiteSpace(descriptionEn) ? null : descriptionEn.Trim();

        MarkAsUpdated(utcNow, userID);

        return Result.Success();
    }  
    
    private static Result ValidateNames(string nameAr, string
        nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
            return Errors.Category.NameArRequired;

        if (string.IsNullOrWhiteSpace(nameEn))
            return Errors.Category.NameEnRequired;

        if (nameAr.Trim().Length > 100)
            return Errors.Category.NameArTooLong;

        if (nameEn.Trim().Length > 100)
            return Errors.Category.NameEnTooLong;

        return Result.Success();
    }
}
