using System;
using Abp.Application.Services.Dto;

namespace ExpenseCategoryApp.ExpenseItems.Dto
{
    public class PagedExpenseItemResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? UserId { get; set; }
        public long? ExpenseCategoryId { get; set; }
        public string Title { get; set; }
        public decimal? Amount { get; set; }
        public int? Currency { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string Notes { get; set; }
        public int? PaymentMethod { get; set; }
        public bool? IsRecurring { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public DateTime? ExpenseDateFrom { get; set; }
        public DateTime? ExpenseDateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CurrencyIn { get; set; }
        public int? CurrencyNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string PaymentMethodIn { get; set; }
        public int? PaymentMethodNot { get; set; }
    }
}
