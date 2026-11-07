using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities;

public class Donation : SoftDeletableEntity
{
    public Guid CampaignId { get; private set; }
    public Guid DonorUserId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public DonationStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }
    public int ReceiptNumber { get; private set; }
    public string? ReceiptFileName { get; private set; }

    private Donation() { }

    public static Donation Create(Guid campaignId, Guid donorUserId, Money amount, DateTime utcNow, Guid? createdBy)
    {
        if (amount is null)
            throw new ArgumentNullException(nameof(amount));

        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (campaignId == Guid.Empty || donorUserId == Guid.Empty)
            throw new ArgumentException("Campaign and donor are required.");

        return new Donation
        {
            CampaignId = campaignId,
            DonorUserId = donorUserId,
            Amount = amount,
            Status = DonationStatus.Pending,
            CreatedAt = utcNow,
            CreatedBy = createdBy
        };
    }

    public void MarkSucceeded(DateTime utcNow, int receiptNumber,Guid? userId, string? receiptFileName = null)
    {
        if (Status != DonationStatus.Pending)
            throw new InvalidOperationException("Only pending donations can succeed.");

        Status = DonationStatus.Succeeded;
        CompletedAt = utcNow;
        ReceiptNumber = receiptNumber;
        ReceiptFileName = receiptFileName;

        MarkAsUpdated(utcNow, userId);
    }

    public void MarkFailed(string reason, DateTime utcNow, Guid? userId)
    {
        if (Status != DonationStatus.Pending)
            throw new InvalidOperationException("Only pending donations can fail.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason is required.");

        Status = DonationStatus.Failed;
        FailureReason = reason.Trim();

        MarkAsUpdated(utcNow, userId);
    }
}