using System;
using Xunit;
using FluentAssertions;
using ExpenseCategoryApp.Entities;

namespace ExpenseCategoryApp.Tests.ExpenseItems
{
    public class ExpenseItemEntityTests
    {
        [Fact]
        public void ExpenseItem_ShouldBeCreatable()
        {
            // Act
            var entity = new ExpenseItem();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void ExpenseItem_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new ExpenseItem();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsRecurring.Should().Be(false);
        }

        [Fact]
        public void ExpenseItem_Title_ShouldAcceptValue()
        {
            var entity = new ExpenseItem { Title = "Test Value" };
            entity.Title.Should().Be("Test Value");
        }

    }
}
