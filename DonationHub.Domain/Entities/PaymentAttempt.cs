using DonationHub.Domain.Common;
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

    public static PaymentAttempt Create(Guid donationId, Money amount, DateTime utcNow, Guid? createdBy)
    {
        if (donationId == Guid.Empty)
            throw new ArgumentException("Donation is required.");

        if (amount is null)
            throw new ArgumentNullException(nameof(amount));

        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount.Amount));

        return new PaymentAttempt
        {
            DonationId = donationId,
            Amount = amount,
            Status = PaymentAttemptStatus.Created,
            CreatedAt = utcNow,
            CreatedBy = createdBy
        };
    }

    public void MarkSucceeded(string paymentIntentId, DateTime utcNow, Guid? UserId)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
            throw new ArgumentException("Payment intent ID is required.");

        IsCreated();

        StripePaymentIntentId = paymentIntentId.Trim();
        Status = PaymentAttemptStatus.Succeeded;
        MarkAsUpdated(utcNow, UserId);
    }

    public void MarkFailed(string failureCode, DateTime utcNow, Guid? UserId)
    {
        IsCreated();

        if (string.IsNullOrWhiteSpace(failureCode))
            throw new ArgumentException("Failure code is required.");

        Status = PaymentAttemptStatus.Failed;
        FailureCode = failureCode.Trim();
        MarkAsUpdated(utcNow, UserId);
    }

    public void MarkExpired(DateTime utcNow, Guid? UserId)
    {
       IsCreated();

        Status = PaymentAttemptStatus.Expired;
        MarkAsUpdated(utcNow, UserId);
    }

    private void IsCreated()
    {
        if (Status != PaymentAttemptStatus.Created)
            throw new InvalidOperationException("Payment attempt is not in a created state.");
    }

    public void SetStripeCheckoutSessionId(string sessionId, DateTime utcNow, Guid? UserId)
    {
        IsCreated();

        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentException("Stripe checkout session ID is required.");

        if (!string.IsNullOrWhiteSpace(StripeCheckoutSessionId))
            throw new InvalidOperationException("Stripe checkout session ID has already been set.");

        StripeCheckoutSessionId = sessionId.Trim();
        MarkAsUpdated(utcNow, UserId);
    }
}