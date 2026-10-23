using System;
using Xunit;
using FluentAssertions;
using ExpenseCategoryApp.Entities;

namespace ExpenseCategoryApp.Tests.ExpenseCategorys
{
    public class ExpenseCategoryEntityTests
    {
        [Fact]
        public void ExpenseCategory_ShouldBeCreatable()
        {
            // Act
            var entity = new ExpenseCategory();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void ExpenseCategory_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new ExpenseCategory();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsActive.Should().Be(false);
        }

        [Fact]
        public void ExpenseCategory_Name_ShouldAcceptValue()
        {
            var entity = new ExpenseCategory { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
