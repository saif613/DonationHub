using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Helper;
using DonationHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities;

public class Campaign : SoftDeletableEntity
{
    public string TitleAr { get; private set; } = string.Empty;
    public string TitleEn { get; private set; } = string.Empty;
    public string DescriptionAr { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;
    public Money GoalAmount { get; private set; } = null!;
    public decimal RaisedAmount { get; private set; } 
    public DateTime EndDate { get; private set; }
    public Guid CategoryId { get; private set; }
    public string? BannerFileName { get; private set; }
    public CampaignStatus Status { get; private set; }  
    public byte[] RowVersion { get; private set; } = [];

    private Campaign() { }

    public static Campaign Create(
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        Money goalAmount,
        DateTime endDate,
        Guid categoryId,
        DateTime utcNow,
        Guid? createdBy)
    {

        HelperText.ValidateText(titleAr, titleEn, descriptionAr, descriptionEn);

        if (goalAmount is null)
            throw new ArgumentNullException(nameof(goalAmount));

        if (goalAmount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(goalAmount));

        if (endDate <= utcNow)
            throw new ArgumentException("End date must be in the future.");

        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category id is required.");

        if (createdBy is null || createdBy == Guid.Empty)
            throw new ArgumentException("Creator id is required.");

        return new Campaign
        {
            TitleAr = titleAr.Trim(),
            TitleEn = titleEn.Trim(),
            DescriptionAr = descriptionAr.Trim(),
            DescriptionEn = descriptionEn.Trim(),
            GoalAmount = goalAmount,
            RaisedAmount = 0,
            EndDate = endDate,
            CategoryId = categoryId,
            CreatedAt = utcNow,
            CreatedBy = createdBy,
            Status = CampaignStatus.Active
        };
    }

    public void ChangeGoal(Money newGoalAmount, DateTime utcNow, Guid? userId)
    {
        if (newGoalAmount is null)
            throw new ArgumentNullException(nameof(newGoalAmount));

        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("Only active campaigns can change their goal.");
  
        if (newGoalAmount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(newGoalAmount));

        if (newGoalAmount.Currency != GoalAmount.Currency)
            throw new InvalidOperationException("Campaign currency cannot be changed.");

        if (newGoalAmount.Amount < RaisedAmount)
            throw new ArgumentOutOfRangeException(nameof(newGoalAmount));

        GoalAmount = newGoalAmount;
        MarkAsUpdated(utcNow, userId);
    }

    public void ChangeEndDate(DateTime newEndDate, DateTime utcNow, Guid? userId)
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("Only active campaigns can change their end date.");

        if (newEndDate <= utcNow)
            throw new ArgumentException("End date must be in the future.");

        EndDate = newEndDate;
        MarkAsUpdated(utcNow, userId);
    }

    public bool CanAcceptDonation(DateTime utcNow) => !IsDeleted && Status == CampaignStatus.Active && EndDate > utcNow;

    public void RegisterSuccessfulDonation(Money amount, DateTime utcNow, Guid? userId)
    {
        if (amount is null)
            throw new ArgumentNullException(nameof(amount));

        if (!CanAcceptDonation(utcNow))
            throw new InvalidOperationException("Campaign cannot accept donations.");

        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (amount.Currency != GoalAmount.Currency)
            throw new InvalidOperationException("Donation currency must match campaign currency.");

        RaisedAmount += amount.Amount;

        if (RaisedAmount >= GoalAmount.Amount)
            Status = CampaignStatus.Completed;

        MarkAsUpdated(utcNow, userId);
    }

    public void Expire(DateTime utcNow, Guid? userId)
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("Only active campaigns can expire.");

        if (EndDate > utcNow)
            throw new InvalidOperationException("Campaign end date has not been reached.");

        Status = CampaignStatus.Expired;
        MarkAsUpdated(utcNow, userId);
    }
}

