using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseCategorys.Dto
{
    [AutoMapTo(typeof(Entities.ExpenseCategory))]
    public class CreateExpenseCategoryDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

    }
}