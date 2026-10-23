using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseItems.Dto
{
    [AutoMapFrom(typeof(Entities.ExpenseItem))]
    public class ExpenseItemDto : EntityDto<long>
    {
        public string Description { get; set; }

        public DateTime ExpenseDate { get; set; }

        public decimal Amount { get; set; }

        public int Currency { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal? AmountInTRY { get; set; }

        public string ReceiptNo { get; set; }

        public long ExpenseReportId { get; set; }

        public long ExpenseCategoryId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}