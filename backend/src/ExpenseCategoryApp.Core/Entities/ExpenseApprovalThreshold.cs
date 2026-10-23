using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ExpenseCategoryApp.Entities
{
    [Table("ExpenseApprovalThresholds")]
    public class ExpenseApprovalThreshold : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public ExpenseApprovalThresholdCurrency Currency { get; set; }

        public decimal ThresholdAmount { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

    }
}