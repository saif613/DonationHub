using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
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

    public static Result<RefreshToken> Create(Guid userId, string tokenHash, DateTime createdAt, DateTime expiresAt, Guid? createdBy)
    {

        if (userId == Guid.Empty)
            return Errors.RefreshToken.UserIdRequired;

        if (string.IsNullOrWhiteSpace(tokenHash))
            return Errors.RefreshToken.TokenHashRequired;

        if (expiresAt <= createdAt)
            return Errors.RefreshToken.InvalidExpiration;

       var refreshToken =  new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = createdAt,
            ExpiresAt = expiresAt,
            CreatedBy = createdBy
        };

        return Result.Success(refreshToken); 
    }
    public static Expression<Func<RefreshToken, bool>> IsActiveExpr(DateTime utcNow) => rt => rt.RevokedAt == null && rt.ExpiresAt > utcNow;

    public bool IsActive(DateTime utcNow) => RevokedAt is null && ExpiresAt > utcNow;

    public Result Revoke(DateTime utcNow, Guid? userId)
    {
        if (RevokedAt is not null)
            return Errors.RefreshToken.AlreadyRevoked;

        if (ExpiresAt <= utcNow)
            return Errors.RefreshToken.Expired;

        RevokedAt = utcNow;
        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }

    public Result ReplaceWith(Guid newTokenId, DateTime utcNow, Guid? userId)
    {
        if (newTokenId == Guid.Empty || newTokenId == Id)
            return Errors.RefreshToken.InvalidReplacementTokenId;

        if (!IsActive(utcNow))
            return Errors.RefreshToken.NotActive;

        RevokedAt = utcNow;
        ReplacedByTokenId = newTokenId;
        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }

}
