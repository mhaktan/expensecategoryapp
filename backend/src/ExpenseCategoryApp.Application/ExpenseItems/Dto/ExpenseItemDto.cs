using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseItems.Dto
{
    [AutoMapFrom(typeof(Entities.ExpenseItem))]
    public class ExpenseItemDto : EntityDto<long>
    {
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public int Currency { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Notes { get; set; }

        public int? PaymentMethod { get; set; }

        public bool? IsRecurring { get; set; }

        public long? UserId { get; set; }

        public long ExpenseCategoryId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}