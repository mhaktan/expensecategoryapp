using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseCategorys.Dto
{
    [AutoMapTo(typeof(Entities.ExpenseCategory))]
    public class CreateExpenseCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(7)]
        public string ColorHex { get; set; }

        [MaxLength(50)]
        public string Icon { get; set; }

    }
}