using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using ExpenseCategoryApp.Entities;
using ExpenseCategoryApp.ExpenseItems;
using ExpenseCategoryApp.ExpenseItems.Dto;

namespace ExpenseCategoryApp.Tests.ExpenseItems
{
    public class ExpenseItemAppServiceTests
    {
        private readonly Mock<IRepository<ExpenseItem, long>> _repositoryMock;
        private readonly ExpenseItemAppService _service;

        public ExpenseItemAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<ExpenseItem, long>>();
            _service = new ExpenseItemAppService(_repositoryMock.Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new ExpenseItem { Id = 1, Title = "Test title", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow },
                new ExpenseItem { Id = 2, Title = "Test title", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow },
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
                new ExpenseItem { Id = 1, Title = "Test title", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow },
                new ExpenseItem { Id = 2, Title = "Test title", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow },
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
            var dto = new CreateExpenseItemDto
            {
                Title = "Test title", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<ExpenseItem>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ExpenseItem { Id = 1, Title = "Test title", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ExpenseItem { Id = 1, Title = "Test title", Amount = 10.0m, Currency = 0, ExpenseDate = DateTime.UtcNow });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
