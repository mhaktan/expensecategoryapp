using System;
using Xunit;
using FluentAssertions;
using ExpenseCategoryApp.Entities;

namespace ExpenseCategoryApp.Tests.ExpenseApprovalThresholds
{
    public class ExpenseApprovalThresholdEntityTests
    {
        [Fact]
        public void ExpenseApprovalThreshold_ShouldBeCreatable()
        {
            // Act
            var entity = new ExpenseApprovalThreshold();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void ExpenseApprovalThreshold_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new ExpenseApprovalThreshold();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsActive.Should().Be(false);
        }

        [Fact]
        public void ExpenseApprovalThreshold_Name_ShouldAcceptValue()
        {
            var entity = new ExpenseApprovalThreshold { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
