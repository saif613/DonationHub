using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities;

public class PaymentAttempt : SoftDeletableEntity
{
    public Guid DonationId { get; private set; }

    public string StripeCheckoutSessionId { get; private set; } = string.Empty;

    public string? StripePaymentIntentId { get; private set; }

    public Money Amount { get; private set; } = null!;

    public PaymentAttemptStatus Status { get; private set; }

    public string? FailureCode { get; private set; }

    private PaymentAttempt() { }

    public static Result<PaymentAttempt> Create(Guid donationId, Money amount, DateTime utcNow, Guid? createdBy)
    {
        if (donationId == Guid.Empty)
            return Errors.Payment.DonationisIdRequired;

        var paymentAttemp = new PaymentAttempt
        {
            DonationId = donationId,
            Amount = amount,
            Status = PaymentAttemptStatus.Created,
            CreatedAt = utcNow,
            CreatedBy = createdBy
        };

        return Result.Success(paymentAttemp);
    }

    public Result MarkSucceeded(string paymentIntentId, DateTime utcNow, Guid? UserId)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
            return Errors.Payment.PaymentIntentIdRequired;

        var stateResult = IsCreated();

        if (stateResult.IsFailure)
            return stateResult;

        StripePaymentIntentId = paymentIntentId.Trim();
        Status = PaymentAttemptStatus.Succeeded;
        MarkAsUpdated(utcNow, UserId);

        return Result.Success();
    }

    public Result MarkFailed(string failureCode, DateTime utcNow, Guid? UserId)
    {
        var stateResult = IsCreated();

        if (stateResult.IsFailure)
            return stateResult;

        if (string.IsNullOrWhiteSpace(failureCode))
            return Errors.Payment.PaymentAttemptFailureCodeRequired;

        Status = PaymentAttemptStatus.Failed;
        FailureCode = failureCode.Trim();
        MarkAsUpdated(utcNow, UserId);

        return Result.Success();
    }

    public Result MarkExpired(DateTime utcNow, Guid? UserId)
    {
        var stateResult = IsCreated();

        if (stateResult.IsFailure)
            return stateResult;

        Status = PaymentAttemptStatus.Expired;
        MarkAsUpdated(utcNow, UserId);

        return Result.Success();
    }

    private Result IsCreated()
    {
        if (Status != PaymentAttemptStatus.Created)
            return Errors.Payment.PaymentAttemptInvalidState;

        return Result.Success();
    }

    public Result SetStripeCheckoutSessionId(string sessionId, DateTime utcNow, Guid? UserId)
    {
        var stateResult = IsCreated();

        if (stateResult.IsFailure)
            return stateResult; 

        if (string.IsNullOrWhiteSpace(sessionId))
            return Errors.Payment.PaymentAttemptCheckoutSessionIdRequired;

        if (!string.IsNullOrWhiteSpace(StripeCheckoutSessionId))
            return Errors.Payment.PaymentAttemptCheckoutSessionAlreadySet;

        StripeCheckoutSessionId = sessionId.Trim();
        MarkAsUpdated(utcNow, UserId);

        return Result.Success();
    }
}