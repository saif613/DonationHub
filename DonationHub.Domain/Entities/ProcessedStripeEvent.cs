using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities;

public class ProcessedStripeEvent 
{
    public string EventId { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;

    public DateTime ProcessedAt { get; private set; }

    private ProcessedStripeEvent() { }

    public static Result<ProcessedStripeEvent> Create(string eventId, string type, DateTime processedAt)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            return Errors.ProcessedStripeEvent.EventIdIsRequired;

        if (string.IsNullOrWhiteSpace(type))
            return Errors.ProcessedStripeEvent.EventTypeIsRequired;

        var processedStripeEvent = new ProcessedStripeEvent
        {
            EventId = eventId.Trim(),
            Type = type.Trim(),
            ProcessedAt = processedAt
        };

        return Result.Success(processedStripeEvent);
    }
}
