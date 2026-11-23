using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
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

    public static Result<Campaign> Create(
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

        var textResult = HelperText.ValidateText(titleAr, titleEn, descriptionAr, descriptionEn);

        if (textResult.IsFailure)
            return textResult.Error;

        if (goalAmount is null)
            return Errors.Campaign.GoalAmountRequired;

        if (endDate <= utcNow)
            return Errors.Campaign.EndDateInPast;

        if (categoryId == Guid.Empty)
            return Errors.Campaign.CategoryIdRequired;

        if (createdBy is null || createdBy == Guid.Empty)
            return Errors.Campaign.CreatorIdRequired;

        var campaign = new Campaign
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

        return Result.Success(campaign);
    }

    public Result ChangeGoal(Money newGoalAmount, DateTime utcNow, Guid? userId)
    {
        if (newGoalAmount is null)
            return Errors.Campaign.GoalAmountRequired;

        if (Status != CampaignStatus.Active)
            return Errors.Campaign.NotActive;

        if (newGoalAmount.Currency != GoalAmount.Currency)
            return Errors.Campaign.CurrencyMismatch;

        if (newGoalAmount.Amount < RaisedAmount)
            return Errors.Campaign.GoalBelowRaisedAmount;

        GoalAmount = newGoalAmount;
        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }

    public Result ChangeEndDate(DateTime newEndDate, DateTime utcNow, Guid? userId)
    {
        if (Status != CampaignStatus.Active)
            return Errors.Campaign.NotActive;

        if (newEndDate <= utcNow)
            return Errors.Campaign.EndDateInPast;

        EndDate = newEndDate;
        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }

    public bool CanAcceptDonation(DateTime utcNow) => !IsDeleted && Status == CampaignStatus.Active && EndDate > utcNow;

    public Result RegisterSuccessfulDonation(Money amount, DateTime utcNow, Guid? userId)
    {
        if (amount is null)
            return Errors.Campaign.GoalAmountRequired;

        if (!CanAcceptDonation(utcNow))
            return Errors.Campaign.CannotAcceptDonation;

        if (amount.Currency != GoalAmount.Currency)
            return Errors.Campaign.CurrencyMismatch;

        RaisedAmount += amount.Amount;

        if (RaisedAmount >= GoalAmount.Amount)
            Status = CampaignStatus.Completed;

        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }

    public Result Expire(DateTime utcNow, Guid? userId)
    {
        if (Status != CampaignStatus.Active)
            return Errors.Campaign.NotActive;

        if (EndDate > utcNow)
            return Errors.Campaign.EndDateNotReached;

        Status = CampaignStatus.Expired;
        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }

    public Result UpdateDetails(string titleAr, string titleEn, string descriptionAr, string descriptionEn, Guid categoryId, DateTime utcNow, Guid? userId)
    {
        if (Status != CampaignStatus.Active)
            return Errors.Campaign.NotActive;

        var textResult = HelperText.ValidateText(titleAr, titleEn, descriptionAr, descriptionEn);

        if (textResult.IsFailure)
            return textResult.Error;

        if (categoryId == Guid.Empty)
            return Errors.Campaign.CategoryIdRequired;

        TitleAr = titleAr.Trim();
        TitleEn = titleEn.Trim();
        DescriptionAr = descriptionAr.Trim();
        DescriptionEn = descriptionEn.Trim();
        CategoryId = categoryId;

        MarkAsUpdated(utcNow, userId);

        return Result.Success();
    }
}

