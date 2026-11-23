using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
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

    public static Result<Donation> Create(Guid campaignId, Guid donorUserId, Money amount, DateTime utcNow, Guid? createdBy)
    {
        if (campaignId == Guid.Empty)
            return Errors.Donation.CampaignIdRequired;

        if (donorUserId == Guid.Empty)
            return Errors.Donation.DonorUserIdRequired;

        if (amount is null)
            return Errors.Donation.AmountRequired;

        var donation = new Donation
        {
            CampaignId = campaignId,
            DonorUserId = donorUserId,
            Amount = amount,
            Status = DonationStatus.Pending,
            CreatedAt = utcNow,
            CreatedBy = createdBy
        };

        return Result.Success(donation); 
    }

    public Result MarkSucceeded(DateTime utcNow, int receiptNumber,Guid? userId, string? receiptFileName = null)
    {
        if (Status != DonationStatus.Pending)
            return Errors.Donation.InvalidState;

        Status = DonationStatus.Succeeded;
        CompletedAt = utcNow;
        ReceiptNumber = receiptNumber;
        ReceiptFileName = receiptFileName;

        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }

    public Result MarkFailed(string reason, DateTime utcNow, Guid? userId)
    {
        if (Status != DonationStatus.Pending)
            return Errors.Donation.InvalidState;

        if (string.IsNullOrWhiteSpace(reason))
            return Errors.Donation.FailureReasonRequired;

        Status = DonationStatus.Failed;
        FailureReason = reason.Trim();

        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }
}