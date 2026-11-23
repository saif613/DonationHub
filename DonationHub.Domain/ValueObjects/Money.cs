using DonationHub.Domain.Common;
using DonationHub.Domain.Common.Results;
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

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (amount <= 0)
            return Errors.Money.InvalidAmount;

        if (string.IsNullOrWhiteSpace(currency))
            return Errors.Money.CurrencyRequired;

        currency = currency.Trim().ToUpperInvariant();

        if (currency.Length != 3)
            return Errors.Money.InvalidCurrency;

        return new Money(amount, currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
