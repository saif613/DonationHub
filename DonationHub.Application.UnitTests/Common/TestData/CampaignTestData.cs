using DonationHub.Application.Features.Auth.Commands.Register;
using DonationHub.Application.Features.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign;
using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObjects;
using Microsoft.Extensions.Time.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.UnitTests.Common.TestData
{
    public class CampaignTestData
    {
        public FakeTimeProvider TimeProvider { get; }

        public DateTime Now => TimeProvider.GetUtcNow().UtcDateTime;

        public DateTime FutureDate => Now.AddDays(30);

        public Guid CategoryId { get; } = Guid.NewGuid();

        public Guid CreatorId { get; } = Guid.NewGuid();

        public Money GoalAmount { get; } = Money.Create(10000m, "EGP");
        public byte[] RowVersion { get; } = [1, 2, 3, 4];

        public CampaignTestData()
        {
            var initialTime = new DateTimeOffset(2026, 11, 5, 10, 0, 0, TimeSpan.Zero);

            TimeProvider = new FakeTimeProvider(initialTime);
        }

        public Campaign CreateCampaign()
        {
            return Campaign.Create(
                "حملة علاج المرضى",
                "Patients Treatment",
                "وصف الحملة",
                "Campaign Description",
                GoalAmount,
                FutureDate,
                CategoryId,
                Now,
                CreatorId);
        }
        public Category CreateCategory()
        {
            return Category.Create(
                "التبرعات",
                "Donations",
                "وصف التصنيف",
                "Category Description",
                Now,
                CreatorId);
        }
        public CreateCampaignCommand CreateCampaignCommand()
        {
            return new CreateCampaignCommand(
                "حملة علاج المرضى",
                "Patients Treatment",
                "وصف الحملة",
                "Campaign Description",
                10000m,
                "EGP",
                FutureDate,
                CategoryId);
        }

        public UpdateCampaignCommand CreateUpdateCampaignCommand(Guid campaignId)
        {
            return new UpdateCampaignCommand(
                campaignId,
                "عنوان جديد",
                "New Title",
                "وصف جديد",
                "New Description",
                20000m,
                "EGP",
                FutureDate.AddDays(10),
                CategoryId,
                RowVersion);
        }
    }
}
