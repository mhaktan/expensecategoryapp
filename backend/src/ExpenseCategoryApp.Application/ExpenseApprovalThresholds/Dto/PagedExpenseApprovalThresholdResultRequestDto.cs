using System;
using Abp.Application.Services.Dto;

namespace ExpenseCategoryApp.ExpenseApprovalThresholds.Dto
{
    public class PagedExpenseApprovalThresholdResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public int? Currency { get; set; }
        public decimal? ThresholdAmount { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public decimal? ThresholdAmountFrom { get; set; }
        public decimal? ThresholdAmountTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CurrencyIn { get; set; }
        public int? CurrencyNot { get; set; }
    }
}
