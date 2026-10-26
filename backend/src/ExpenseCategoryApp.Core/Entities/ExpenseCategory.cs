using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ExpenseCategoryApp.Entities
{
    [Table("ExpenseCategorys")]
    public class ExpenseCategory : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(7)]
        public string ColorHex { get; set; }

        [MaxLength(50)]
        public string Icon { get; set; }

        public virtual ICollection<ExpenseItem> ExpenseItems { get; set; }

        public virtual ICollection<Budget> Budgets { get; set; }

    }
}