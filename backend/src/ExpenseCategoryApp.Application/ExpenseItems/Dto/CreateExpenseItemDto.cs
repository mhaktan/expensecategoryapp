using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseItems.Dto
{
    [AutoMapTo(typeof(Entities.ExpenseItem))]
    public class CreateExpenseItemDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public int Currency { get; set; }

        public DateTime ExpenseDate { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public int? PaymentMethod { get; set; }

        public bool? IsRecurring { get; set; }

        public long? UserId { get; set; }

        public long ExpenseCategoryId { get; set; }

    }
}