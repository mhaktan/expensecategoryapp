using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.Budgets.Dto
{
    [AutoMapTo(typeof(Entities.Budget))]
    public class CreateBudgetDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public decimal LimitAmount { get; set; }

        public int Currency { get; set; }

        public int PeriodType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? WarningThresholdPercent { get; set; }

        public long? UserId { get; set; }

        public long ExpenseCategoryId { get; set; }

    }
}