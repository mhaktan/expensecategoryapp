using System;
using Xunit;
using FluentAssertions;
using ExpenseCategoryApp.Entities;

namespace ExpenseCategoryApp.Tests.ExpenseReports
{
    public class ExpenseReportEntityTests
    {
        [Fact]
        public void ExpenseReport_ShouldBeCreatable()
        {
            // Act
            var entity = new ExpenseReport();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void ExpenseReport_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new ExpenseReport();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void ExpenseReport_Title_ShouldAcceptValue()
        {
            var entity = new ExpenseReport { Title = "Test Value" };
            entity.Title.Should().Be("Test Value");
        }

    }
}
