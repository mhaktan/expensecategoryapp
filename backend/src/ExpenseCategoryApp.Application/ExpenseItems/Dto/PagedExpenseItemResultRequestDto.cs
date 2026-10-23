using System;
using Abp.Application.Services.Dto;

namespace ExpenseCategoryApp.ExpenseItems.Dto
{
    public class PagedExpenseItemResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? ExpenseReportId { get; set; }
        public long? ExpenseCategoryId { get; set; }
        public string Description { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public decimal? Amount { get; set; }
        public int? Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? AmountInTRY { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? ExpenseDateFrom { get; set; }
        public DateTime? ExpenseDateTo { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public decimal? ExchangeRateFrom { get; set; }
        public decimal? ExchangeRateTo { get; set; }
        public decimal? AmountInTRYFrom { get; set; }
        public decimal? AmountInTRYTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CurrencyIn { get; set; }
        public int? CurrencyNot { get; set; }
    }
}
