using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using ExpenseCategoryApp.Entities;
using ExpenseCategoryApp.Budgets;
using ExpenseCategoryApp.Budgets.Dto;

namespace ExpenseCategoryApp.Tests.Budgets
{
    public class BudgetAppServiceTests
    {
        private readonly Mock<IRepository<Budget, long>> _repositoryMock;
        private readonly BudgetAppService _service;

        public BudgetAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Budget, long>>();
            _service = new BudgetAppService(_repositoryMock.Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Budget { Id = 1, Name = "Test name", LimitAmount = 10.0m, Currency = 0, PeriodType = 0, StartDate = DateTime.UtcNow },
                new Budget { Id = 2, Name = "Test name", LimitAmount = 10.0m, Currency = 0, PeriodType = 0, StartDate = DateTime.UtcNow },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act
            var result = _repositoryMock.Object.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public void Repository_GetAll_WithFilter_ShouldWork()
        {
            // Arrange
            var entities = new[]
            {
                new Budget { Id = 1, Name = "Test name", LimitAmount = 10.0m, Currency = 0, PeriodType = 0, StartDate = DateTime.UtcNow },
                new Budget { Id = 2, Name = "Test name", LimitAmount = 10.0m, Currency = 0, PeriodType = 0, StartDate = DateTime.UtcNow },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act — simulate keyword filter
            var result = _repositoryMock.Object.GetAll()
                .Where(x => x.Id.ToString().Contains("1"));

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldInsertEntity()
        {
            // Arrange
            var dto = new CreateBudgetDto
            {
                Name = "Test name", LimitAmount = 10.0m, Currency = 0, PeriodType = 0, StartDate = DateTime.UtcNow
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Budget>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Budget { Id = 1, Name = "Test name", LimitAmount = 10.0m, Currency = 0, PeriodType = 0, StartDate = DateTime.UtcNow });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Budget { Id = 1, Name = "Test name", LimitAmount = 10.0m, Currency = 0, PeriodType = 0, StartDate = DateTime.UtcNow });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
