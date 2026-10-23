using System;
using ExpenseCategoryApp.Analytics.Dto;

namespace ExpenseCategoryApp.ExpenseReports.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class ExpenseReportGroupedCountInput : PagedExpenseReportResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class ExpenseReportStatsInput : PagedExpenseReportResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
