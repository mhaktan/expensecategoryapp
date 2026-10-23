using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseItems.Dto
{
    [AutoMapTo(typeof(Entities.ExpenseItem))]
    public class CreateExpenseItemDto
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime ExpenseDate { get; set; }

        public decimal Amount { get; set; }

        public int Currency { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? AmountInTRY { get; set; }

        [MaxLength(100)]
        public string ReceiptNo { get; set; }

        public long ExpenseReportId { get; set; }

        public long ExpenseCategoryId { get; set; }

    }
}