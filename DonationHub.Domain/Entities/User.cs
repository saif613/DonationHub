using DonationHub.Domain.Common;
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

    public static User Create(Guid id, string fullName, string preferredLanguage, DateTime utcNow, Guid? createdBy)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.");

        if (preferredLanguage is not ("en" or "ar"))
            throw new ArgumentException("Preferred language must be en or ar.");

        return new User
        {
            Id = id,
            FullName = fullName.Trim(),
            PreferredLanguage = preferredLanguage.Trim(),
            IsActive = true,
            CreatedAt = utcNow,
            CreatedBy = createdBy
        };
    }

    public void ChangePreferredLanguage(string language, DateTime utcNow, Guid? userId)
    {
        if (language is not ("en" or "ar"))
            throw new ArgumentException("Preferred language must be en or ar.");

        PreferredLanguage = language;
        MarkAsUpdated(utcNow, userId);
    }

    public void Deactivate(Guid? userId, DateTime utcNow)
    {
        if (!IsActive)
            throw new InvalidOperationException("User is already deactivated.");

        IsActive = false;
        MarkAsUpdated(utcNow, userId);
    }

    public void Activate(Guid? userId, DateTime utcNow)
    {
        if (IsActive)
            throw new InvalidOperationException("User is already activated.");

        IsActive = true;
        MarkAsUpdated(utcNow, userId);
    }
    public void UpdateProfile(string fullName, DateTime utcNow, Guid? userId)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new AggregateException("Full name is required");

        FullName = fullName.Trim();

        MarkAsUpdated(utcNow, userId);
    }
}
