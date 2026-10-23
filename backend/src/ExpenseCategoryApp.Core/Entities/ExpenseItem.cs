using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ExpenseCategoryApp.Entities
{
    [Table("ExpenseItems")]
    public class ExpenseItem : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime ExpenseDate { get; set; }

        public decimal Amount { get; set; }

        public ExpenseItemCurrency Currency { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? AmountInTRY { get; set; }

        [MaxLength(100)]
        public string ReceiptNo { get; set; }

        public long ExpenseReportId { get; set; }

        [ForeignKey(nameof(ExpenseReportId))]
        public virtual ExpenseReport ExpenseReport { get; set; }

        public long ExpenseCategoryId { get; set; }

        [ForeignKey(nameof(ExpenseCategoryId))]
        public virtual ExpenseCategory ExpenseCategory { get; set; }

    }
}