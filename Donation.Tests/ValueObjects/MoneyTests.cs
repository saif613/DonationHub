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

            var money = Money.Create(amount, currency);

            money.Should().NotBeNull();
            money.Amount.Should().Be(amount);
            money.Currency.Should().Be(currency);
        }

        [Fact]
        public void Create_WithNegativeAmount_ShouldThrowException()
        {
            var act = () => Money.Create(-100m, "EGP");

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("amount");
        }

        [Fact]
        public void Create_WithZeroAmount_ShouldThrowException()
        {
            var act = () => Money.Create(0m, "EGP");

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("amount");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_WithInvalidCurrency_ShouldThrowException(string? currency)
        {
            var act = () => Money.Create(100m, currency!);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Currency is required.");
        }

        [Theory]
        [InlineData("US")]
        [InlineData("USDD")]
        [InlineData("E")]
        public void Create_WithInvalidCurrencyLength_ShouldThrowException(string currency)
        {
            var act = () => Money.Create(100m, currency);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Currency must be a three-letter code.");
        }

        [Fact]
        public void Create_WithLowerCaseCurrency_ShouldNormalizedCurrency()
        {
            var currency = "  egp  ";

            var money = Money.Create(100m, currency);

            money.Currency.Should().Be("EGP");
        }

        [Fact]
        public void Equals_WithSameValues_ShouldReturnTrue()
        {
            var first = Money.Create(100m, "EGP");
            var second = Money.Create(100m, "EGP");

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
            var first = Money.Create(100m, "EGP");
            var second = Money.Create(100m, "USD");

            first.Should().NotBe(second);
        }

        [Fact]
        public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
        {
            var first = Money.Create(100m, "EGP");
            var second = Money.Create(100m, "EGP");

            first.GetHashCode().Should().Be(second.GetHashCode());
        }
    }
}
