using DonationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities;

public class IdempotencyRecord : SoftDeletableEntity
{
    public Guid UserId { get; private set; }

    public string RequestHash { get; private set; } = string.Empty;

    public int ResponseStatusCode { get; private set; }

    public string ResponseBody { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    private IdempotencyRecord() { }

    public static IdempotencyRecord Create(
        Guid userId,
        string requestHash,
        int responseStatusCode,
        string responseBody,
        DateTime createdAt,
        DateTime expiresAt,
        Guid? createdBy)
    {

        if (userId == Guid.Empty)
            throw new ArgumentException("User is required.");

        if (string.IsNullOrWhiteSpace(requestHash))
            throw new ArgumentException("Request hash is required.");

        if (expiresAt <= createdAt)
            throw new ArgumentException("Expiration must be after creation.");

        if (responseBody is null)
            throw new ArgumentNullException(nameof(responseBody));

        return new IdempotencyRecord
        {
            UserId = userId,
            RequestHash = requestHash,
            ResponseStatusCode = responseStatusCode,
            ResponseBody = responseBody,
            CreatedAt = createdAt,
            ExpiresAt = expiresAt,
            CreatedBy = createdBy
        };
    }

    public bool IsExpired(DateTime utcNow) => ExpiresAt <= utcNow;
}
