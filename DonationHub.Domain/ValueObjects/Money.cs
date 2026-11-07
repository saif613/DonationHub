using DonationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.");

        currency = currency.Trim().ToUpperInvariant();

        if (currency.Length != 3)
            throw new ArgumentException("Currency must be a three-letter code.");

        return new Money(amount, currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
