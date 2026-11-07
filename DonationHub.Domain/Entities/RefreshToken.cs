using DonationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DonationHub.Domain.Entities;

public class RefreshToken : SoftDeletableEntity
{
    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string tokenHash, DateTime createdAt, DateTime expiresAt, Guid? createdBy)
    {

        if (userId == Guid.Empty)
            throw new ArgumentException("User is required.");

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash is required.");

        if (expiresAt <= createdAt)
            throw new ArgumentException("Expiration must be after creation.");

        return new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = createdAt,
            ExpiresAt = expiresAt,
            CreatedBy = createdBy
        };
    }
    public static Expression<Func<RefreshToken, bool>> IsActiveExpr(DateTime utcNow) => rt => rt.RevokedAt == null && rt.ExpiresAt > utcNow;

    public bool IsActive(DateTime utcNow) => RevokedAt is null && ExpiresAt > utcNow;

    public void Revoke(DateTime utcNow, Guid? userId)
    {
        if (RevokedAt is not null)
            throw new InvalidOperationException("Token is already revoked.");

        if (ExpiresAt <= utcNow)
            throw new InvalidOperationException("Token has expired.");

        RevokedAt = utcNow;
        MarkAsUpdated(utcNow, userId);
    }

    public void ReplaceWith(Guid newTokenId, DateTime utcNow, Guid? userId)
    {
        if (newTokenId == Guid.Empty || newTokenId == Id)
            throw new ArgumentException("Replacement token ID is invalid.");

        if (!IsActive(utcNow))
            throw new InvalidOperationException("Token is not active.");

        RevokedAt = utcNow;
        ReplacedByTokenId = newTokenId;
        MarkAsUpdated(utcNow, userId);
    }

}
