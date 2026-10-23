using System;
using Abp.Application.Services.Dto;

namespace ExpenseCategoryApp.ExpenseReports.Dto
{
    public class PagedExpenseReportResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReportDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? Currency { get; set; }
        public int? Status { get; set; }
        public string RevisionNote { get; set; }
        public DateTime? ReportDateFrom { get; set; }
        public DateTime? ReportDateTo { get; set; }
        public decimal? TotalAmountFrom { get; set; }
        public decimal? TotalAmountTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CurrencyIn { get; set; }
        public int? CurrencyNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
