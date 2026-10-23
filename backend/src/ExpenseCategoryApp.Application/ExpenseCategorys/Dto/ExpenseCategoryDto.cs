using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseCategorys.Dto
{
    [AutoMapFrom(typeof(Entities.ExpenseCategory))]
    public class ExpenseCategoryDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}