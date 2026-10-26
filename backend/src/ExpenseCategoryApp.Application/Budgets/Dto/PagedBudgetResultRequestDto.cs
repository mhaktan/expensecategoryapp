using System;
using Abp.Application.Services.Dto;

namespace ExpenseCategoryApp.Budgets.Dto
{
    public class PagedBudgetResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? UserId { get; set; }
        public long? ExpenseCategoryId { get; set; }
        public string Name { get; set; }
        public decimal? LimitAmount { get; set; }
        public int? Currency { get; set; }
        public int? PeriodType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? WarningThresholdPercent { get; set; }
        public decimal? LimitAmountFrom { get; set; }
        public decimal? LimitAmountTo { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public int? WarningThresholdPercentFrom { get; set; }
        public int? WarningThresholdPercentTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CurrencyIn { get; set; }
        public int? CurrencyNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string PeriodTypeIn { get; set; }
        public int? PeriodTypeNot { get; set; }
    }
}
