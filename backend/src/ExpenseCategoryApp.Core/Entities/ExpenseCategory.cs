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
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<ExpenseItem> ExpenseItems { get; set; }

    }
}