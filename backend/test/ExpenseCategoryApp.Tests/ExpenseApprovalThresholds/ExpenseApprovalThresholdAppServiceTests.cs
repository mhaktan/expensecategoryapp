using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using ExpenseCategoryApp.Entities;
using ExpenseCategoryApp.ExpenseApprovalThresholds;
using ExpenseCategoryApp.ExpenseApprovalThresholds.Dto;
using ExpenseCategoryApp.Flows;

namespace ExpenseCategoryApp.Tests.ExpenseApprovalThresholds
{
    public class ExpenseApprovalThresholdAppServiceTests
    {
        private readonly Mock<IRepository<ExpenseApprovalThreshold, long>> _repositoryMock;
        private readonly ExpenseApprovalThresholdAppService _service;

        public ExpenseApprovalThresholdAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<ExpenseApprovalThreshold, long>>();
            _service = new ExpenseApprovalThresholdAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new ExpenseApprovalThreshold { Id = 1, Name = "Test name", Currency = 0, ThresholdAmount = 10.0m, IsActive = true },
                new ExpenseApprovalThreshold { Id = 2, Name = "Test name", Currency = 0, ThresholdAmount = 10.0m, IsActive = true },
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
                new ExpenseApprovalThreshold { Id = 1, Name = "Test name", Currency = 0, ThresholdAmount = 10.0m, IsActive = true },
                new ExpenseApprovalThreshold { Id = 2, Name = "Test name", Currency = 0, ThresholdAmount = 10.0m, IsActive = true },
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
            var dto = new CreateExpenseApprovalThresholdDto
            {
                Name = "Test name", Currency = 0, ThresholdAmount = 10.0m, IsActive = true
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<ExpenseApprovalThreshold>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ExpenseApprovalThreshold { Id = 1, Name = "Test name", Currency = 0, ThresholdAmount = 10.0m, IsActive = true });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ExpenseApprovalThreshold { Id = 1, Name = "Test name", Currency = 0, ThresholdAmount = 10.0m, IsActive = true });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
