using System;
using Xunit;
using FluentAssertions;
using ExpenseCategoryApp.Entities;

namespace ExpenseCategoryApp.Tests.Budgets
{
    public class BudgetEntityTests
    {
        [Fact]
        public void Budget_ShouldBeCreatable()
        {
            // Act
            var entity = new Budget();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Budget_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Budget();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Budget_Name_ShouldAcceptValue()
        {
            var entity = new Budget { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
