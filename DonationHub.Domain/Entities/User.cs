using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities;

public class User : SoftDeletableEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string PreferredLanguage { get; private set; } = "en";
    public bool IsActive { get; private set; }
    private User() { }

    public static Result<User> Create(Guid id, string fullName, string preferredLanguage, DateTime utcNow, Guid? createdBy)
    {
        if (id == Guid.Empty)
            return Errors.User.IdRequired;

        if (string.IsNullOrWhiteSpace(fullName))
            return Errors.User.FullNameRequired;

        if (preferredLanguage is not ("en" or "ar"))
            return Errors.User.InvalidLanguage;

        var user = new User
        {
            Id = id,
            FullName = fullName.Trim(),
            PreferredLanguage = preferredLanguage.Trim(),
            IsActive = true,
            CreatedAt = utcNow,
            CreatedBy = createdBy
        };

        return Result.Success(user);
    }

    public Result ChangePreferredLanguage(string language, DateTime utcNow, Guid? userId)
    {
        language = language?.Trim() ?? string.Empty;

        if (language is not ("en" or "ar"))
            return Errors.User.InvalidLanguage;

        PreferredLanguage = language;
        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }

    public Result Deactivate(Guid? userId, DateTime utcNow)
    {
        if (!IsActive)
            return Errors.User.AlreadyDeactivated;

        IsActive = false;
        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }

    public Result Activate(Guid? userId, DateTime utcNow)
    {
        if (IsActive)
            return Errors.User.AlreadyActivated;

        IsActive = true;
        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }
    public Result UpdateProfile(string fullName, DateTime utcNow, Guid? userId)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return Errors.User.FullNameRequired;

        FullName = fullName.Trim();

        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }
}
