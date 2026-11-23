using Donation.Tests.Common;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donation.Tests.EntitiesTest
{    
    public class CategoryTests
    {
        [Fact]
        public void Create_WithVAlidData_ShouldCreateCategory()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = Category.Create(" تبرعات طبية ", " Medical Donations ", " وصف عربي ", " English Description ", TestDateTime.Now, userId);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var category = result.Value;

            category.NameAr.Should().Be("تبرعات طبية");
            category.NameEn.Should().Be("Medical Donations");

            category.DescriptionAr.Should().Be("وصف عربي");
            category.DescriptionEn.Should().Be("English Description");

            category.CreatedAt.Should().Be(TestDateTime.Now);
            category.CreatedBy.Should().Be(userId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_WithInvalidArabicName_ShouldReturnFailure(string? nameAr)
        {
            // Arrange
            var result = Category.Create(nameAr!, "Medical", "", "", TestDateTime.Now, null);

            // Act and Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Category.NameArRequired);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_WithInvalidEnglishName_ShouldReturnFailure(string? nameEn)
        {
            // Arrange
            var result = Category.Create("تبرعات طبية", nameEn!, "", "", TestDateTime.Now, null);

            // Act and Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Category.NameEnRequired);
        }

        [Fact]
        public void Create_WithArabicNameExceeding100Characters_ShouldReturnFailure()
        {
            // Arrnge
            var nameAr = new string('ع', 101);

            var result = Category.Create(nameAr, "Medical", "", "", TestDateTime.Now, null);

            // Act and Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Category.NameArTooLong);
        }

        [Fact]
        public void Create_WithEnglishNameExceeding100Characters_ShouldReturnFailure()
        {
            // Arrnge
            var nameEn = new string('A', 101);

            var result = Category.Create("تبرعات طبي", nameEn, "", "", TestDateTime.Now, null);

            // Act and Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Category.NameEnTooLong);
        }

        [Fact]
        public void Create_WithNamesOf100Characters_ShouldSucceed()
        {
            // Arrange 
            var nameAr = new string('ع', 100);
            var nameEn = new string('A', 100);

            // Act

            var result = Category.Create(nameAr, nameEn, "", "", TestDateTime.Now, null);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.NameAr.Should().HaveLength(100);
            result.Value.NameEn.Should().HaveLength(100);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithEmptyDescriptions_ShouldSetThemToNull(string? description)
        {
            // Arrange and Act
            var result = Category.Create("تبرعات طبية", "Medical", description!, description!, TestDateTime.Now, null);

            // Assert
            result.IsSuccess.Should().BeTrue();

            result.Value.DescriptionAr.Should().BeNull();
            result.Value.DescriptionEn.Should().BeNull();
        }

        [Fact]
        public void Create_WithDescriptionsHavingSpaces_ShouldTrimThem()
        {
            // Arrange & Act
            var result = Category.Create("تبرعات طبية", "Medical", " Arabic Description ", " English Description ", TestDateTime.Now, null);

            // Assert

            result.IsSuccess.Should().BeTrue();

            result.Value.DescriptionAr.Should().Be("Arabic Description");
            result.Value.DescriptionEn.Should().Be("English Description");
        }

        [Fact]
        public void Update_WithValidateData_ShouldUpdateCategory()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var categoryResult = Category.Create("تبرعات طبية", "Medical", "Old Arabic Description", "old English Description", TestDateTime.Now, null);

            categoryResult.IsSuccess.Should().BeTrue();

            var category = categoryResult.Value;

            // Act
           var result = category.Update("تبرعات تعليمية", "Education", "New Arabic Description", "New English Description", TestDateTime.Later, userId);

            // Assert
            result.IsSuccess.Should().BeTrue();

            category.NameAr.Should().Be("تبرعات تعليمية");
            category.NameEn.Should().Be("Education");

            category.DescriptionAr.Should().Be("New Arabic Description");
            category.DescriptionEn.Should().Be("New English Description");

            category.UpdatedAt.Should().Be(TestDateTime.Later);
            category.UpdatedBy.Should().Be(userId);
            category.CreatedAt.Should().Be(TestDateTime.Now);
            category.CreatedBy.Should().BeNull();

        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Update_WithInvalidArabicName_ShouldReturnFailure(string? nameAr)
        {
            // Arrange 
            var userId = Guid.NewGuid();

            var category = Category.Create("تبرعات طبية", "Medical", "Old Description", "", TestDateTime.Now, null).Value;

            var result = category.Update(nameAr!, "Education","New Description", "", TestDateTime.Later, userId);

            // Act 
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Category.NameArRequired);

            // Assert
            category.NameAr.Should().Be("تبرعات طبية");
            category.NameEn.Should().Be("Medical");
            category.DescriptionAr.Should().Be("Old Description");
            category.DescriptionEn.Should().BeNull();
            category.UpdatedAt.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Update_WithInvalidEnglishName_ShouldReturnFailure(string? nameEn)
        {
            // Arrange 
            var userId = Guid.NewGuid();

            var category = Category.Create("تبرعات طبية", "Medical", "Old Description", "", TestDateTime.Now, null).Value;

            var result = category.Update("تبرعات تعليمية", nameEn!, "New Description", "", TestDateTime.Later, userId);

            // Act 
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Errors.Category.NameEnRequired);

            // Assert
            category.NameAr.Should().Be("تبرعات طبية");
            category.NameEn.Should().Be("Medical");
            category.DescriptionAr.Should().Be("Old Description");
            category.DescriptionEn.Should().BeNull();
            category.UpdatedAt.Should().BeNull();
        }
    }
}
