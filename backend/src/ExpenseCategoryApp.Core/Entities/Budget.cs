using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ExpenseCategoryApp.Entities
{
    [Table("Budgets")]
    public class Budget : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public decimal LimitAmount { get; set; }

        public BudgetCurrency Currency { get; set; }

        public BudgetPeriodType PeriodType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? WarningThresholdPercent { get; set; }

        public long? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public long ExpenseCategoryId { get; set; }

        [ForeignKey(nameof(ExpenseCategoryId))]
        public virtual ExpenseCategory ExpenseCategory { get; set; }

    }
}