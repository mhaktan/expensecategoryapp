using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ExpenseCategoryApp.Entities
{
    // State Machine: status — Draft → PendingManagerApproval → PendingFinanceApproval → Approved → Revision → Rejected → Cancelled
    // Initial: Draft | Transitions: Draft→PendingManagerApproval[Submit], PendingManagerApproval→PendingFinanceApproval[Approve], PendingManagerApproval→Revision[Revise], PendingFinanceApproval→Approved[Approve], PendingFinanceApproval→Revision[Revise], Revision→PendingManagerApproval[Resubmit], PendingManagerApproval→Rejected[Reject], PendingFinanceApproval→Rejected[Reject], *→Cancelled[Cancel]
    [Table("ExpenseReports")]
    public class ExpenseReport : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public DateTime ReportDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public ExpenseReportCurrency Currency { get; set; }

        public ExpenseReportStatus Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public virtual ICollection<ExpenseItem> ExpenseItems { get; set; }

    }
}