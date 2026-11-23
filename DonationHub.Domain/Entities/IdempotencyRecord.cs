using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
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

    public static Result<IdempotencyRecord> Create(
        Guid userId,
        string requestHash,
        int responseStatusCode,
        string responseBody,
        DateTime createdAt,
        DateTime expiresAt,
        Guid? createdBy)
    {

        if (userId == Guid.Empty)
            return Errors.Idempotency.UserIdRequired;

        if (string.IsNullOrWhiteSpace(requestHash))
            return Errors.Idempotency.RequestHashRequired;

        if (expiresAt <= createdAt)
            return Errors.Idempotency.InvalidExpiration;

        if (responseBody is null)
            return Errors.Idempotency.ResponseBodyRequired;

        var idempotencyRecord = new IdempotencyRecord
        {
            UserId = userId,
            RequestHash = requestHash,
            ResponseStatusCode = responseStatusCode,
            ResponseBody = responseBody,
            CreatedAt = createdAt,
            ExpiresAt = expiresAt,
            CreatedBy = createdBy
        };

        return Result.Success(idempotencyRecord);
    }

    public bool IsExpired(DateTime utcNow) => ExpiresAt <= utcNow;
}
