using System;
using ExpenseCategoryApp.Analytics.Dto;

namespace ExpenseCategoryApp.ExpenseApprovalThresholds.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class ExpenseApprovalThresholdGroupedCountInput : PagedExpenseApprovalThresholdResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class ExpenseApprovalThresholdStatsInput : PagedExpenseApprovalThresholdResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
