using Donation.Tests.Common;
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
            var category = Category.Create(" تبرعات طبية ", " Medical Donations ", " وصف عربي ", " English Description ", TestDateTime.Now, userId);

            // Assert
            category.Should().NotBeNull();

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
        public void Create_WithInvalidArabicName_ShouldThrowArgumentException(string? nameAr)
        {
            // Arrange
            var act = () => Category.Create(nameAr!, "Medical", "", "", TestDateTime.Now, null);

            // Act and Assert
            act.Should().Throw<ArgumentException>().WithMessage("Arabic Name is Required.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_WithInvalidEnglishName_ShouldThrowArgumentException(string? nameEn)
        {
            // Arrange
            var act = () => Category.Create("تبرعات طبية", nameEn!, "", "", TestDateTime.Now, null);

            // Act and Assert
            act.Should().Throw<ArgumentException>().WithMessage("English Name is Required");
        }

        [Fact]
        public void Create_WithArabicNameExceeding100Characters_ShouldThrowException()
        {
            // Arrnge
            var nameAr = new string('ع', 101);

            var act = () => Category.Create(nameAr, "Medical", "", "", TestDateTime.Now, null);

            // Act and Assert
            act.Should().Throw<ArgumentException>().WithMessage("Arabic name cannot exceed 100 characters");
        }

        [Fact]
        public void Create_WithEnglishNameExceeding100Characters_ShouldThrowException()
        {
            // Arrnge
            var nameEn = new string('A', 101);

            var act = () => Category.Create("تبرعات طبي", nameEn, "", "", TestDateTime.Now, null);

            // Act and Assert
            act.Should().Throw<ArgumentException>().WithMessage("English name cannot exceed 100 characters");
        }

        [Fact]
        public void Create_WithNamesOf100Characters_ShouldSucceed()
        {
            // Arrange 
            var nameAr = new string('ع', 100);
            var nameEn = new string('A', 100);

            // Act

            var category = Category.Create(nameAr, nameEn, "", "", TestDateTime.Now, null);

            // Assert
            category.NameAr.Should().HaveLength(100);
            category.NameEn.Should().HaveLength(100);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithEmptyDescriptions_ShouldSetThemToNull(string? description)
        {
            // Arrange and Act
            var category = Category.Create("تبرعات طبية", "Medical", description!, description!, TestDateTime.Now, null);

            // Assert
            category.DescriptionAr.Should().BeNull();
            category.DescriptionEn.Should().BeNull();
        }

        [Fact]
        public void Create_WithDescriptionsHavingSpaces_ShouldTrimThem()
        {
            // Arrange & Act
            var category = Category.Create("تبرعات طبية", "Medical", " Arabic Description ", " English Description ", TestDateTime.Now, null);

            // Assert
            category.DescriptionAr.Should().Be("Arabic Description");
            category.DescriptionEn.Should().Be("English Description");
        }

        [Fact]
        public void Update_WithValidateData_ShouldUpdateCategory()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var category = Category.Create("تبرعات طبية", "Medical", "Old Arabic Description", "old English Description", TestDateTime.Now, null);

            // Act
            category.Update("تبرعات تعليمية", "Education", "New Arabic Description", "New English Description", TestDateTime.Later, userId);

            // Assert
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
        public void Update_WithInvalidArabicName_ShouldThrowException(string? nameAr)
        {
            // Arrange 
            var userId = Guid.NewGuid();

            var category = Category.Create("تبرعات طبية", "Medical", "Old Description", "", TestDateTime.Now, null);

            var act = () => category.Update(nameAr!, "Education","New Description", "", TestDateTime.Later, userId);

            // Act 
            act.Should().Throw<ArgumentException>();

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
        public void Update_WithInvalidEnglishName_ShouldThrowException(string? nameEn)
        {
            // Arrange 
            var userId = Guid.NewGuid();

            var category = Category.Create("تبرعات طبية", "Medical", "Old Description", "", TestDateTime.Now, null);

            var act = () => category.Update("تبرعات تعليمية", nameEn!, "New Description", "", TestDateTime.Later, userId);

            // Act 
            act.Should().Throw<ArgumentException>();

            // Assert
            category.NameAr.Should().Be("تبرعات طبية");
            category.NameEn.Should().Be("Medical");
            category.DescriptionAr.Should().Be("Old Description");
            category.DescriptionEn.Should().BeNull();
            category.UpdatedAt.Should().BeNull();
        }
    }
}
