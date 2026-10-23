using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseApprovalThresholds.Dto
{
    [AutoMapTo(typeof(Entities.ExpenseApprovalThreshold))]
    public class CreateExpenseApprovalThresholdDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public int Currency { get; set; }

        public decimal ThresholdAmount { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

    }
}