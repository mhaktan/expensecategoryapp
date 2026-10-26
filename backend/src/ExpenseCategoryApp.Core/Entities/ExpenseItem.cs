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
        [MaxLength(200)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public ExpenseItemCurrency Currency { get; set; }

        public DateTime ExpenseDate { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public ExpenseItemPaymentMethod? PaymentMethod { get; set; }

        public bool? IsRecurring { get; set; }

        public long? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public long ExpenseCategoryId { get; set; }

        [ForeignKey(nameof(ExpenseCategoryId))]
        public virtual ExpenseCategory ExpenseCategory { get; set; }

    }
}