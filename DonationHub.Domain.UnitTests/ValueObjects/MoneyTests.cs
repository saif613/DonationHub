using DonationHub.Domain.Common.Results;
using DonationHub.Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donation.Tests.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Create_withValidData_ShouldCreateMoney()
        {
            var amount = 100;
            var currency = "EGP";

            var result = Money.Create(amount, currency);

            result.IsSuccess.Should().BeTrue();
            result.Value.Amount.Should().Be(amount);
            result.Value.Currency.Should().Be(currency);
        }

        [Fact]
        public void Create_WithNegativeAmount_ShouldReturnInvalidAmountError()
        {
            var result = Money.Create(-100m, "EGP");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Money.InvalidAmount);
        }

        [Fact]
        public void Create_WithZeroAmount_ShouldReturnInvalidAmountError()
        {
            var result = Money.Create(0m, "EGP");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Money.InvalidAmount);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_WithInvalidCurrency_ShouldReturnCurrencyRequiredError(string? currency)
        {
            var result = Money.Create(100m, currency!);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Money.CurrencyRequired);
        }

        [Theory]
        [InlineData("US")]
        [InlineData("USDD")]
        [InlineData("E")]
        public void Create_WithInvalidCurrencyLength_ShouldReturnInvalidCurrencyError(string currency)
        {
            var result = Money.Create(100m, currency);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Money.InvalidCurrency);
        }

        [Fact]
        public void Create_WithLowerCaseCurrency_ShouldNormalizedCurrency()
        {
            var result = Money.Create(100m, "  egp  ");

            result.IsSuccess.Should().BeTrue();
            result.Value.Currency.Should().Be("EGP");
        }

        [Fact]
        public void Equals_WithSameValues_ShouldReturnTrue()
        {
            var first = Money.Create(100m, "EGP").Value;
            var second = Money.Create(100m, "EGP").Value;

            first.Should().Be(second);
        }

        [Fact]
        public void Equals_WithDifferentAmounts_ShouldReturnFalse()
        {
            var first = Money.Create(100m, "EGP");
            var second = Money.Create(200m, "EGP");

            first.Should().NotBe(second);
        }

        [Fact]
        public void Equals_WithDifferentCurrencies_ShouldReturnFalse()
        {
            var first = Money.Create(100m, "EGP").Value;
            var second = Money.Create(100m, "USD").Value;

            first.Should().NotBe(second);
        }

        [Fact]
        public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
        {
            var first = Money.Create(100m, "EGP").Value;
            var second = Money.Create(100m, "EGP").Value;

            first.GetHashCode().Should().Be(second.GetHashCode());
        }
    }
}
