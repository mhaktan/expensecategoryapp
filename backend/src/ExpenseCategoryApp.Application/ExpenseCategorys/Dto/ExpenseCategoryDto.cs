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

        public string ColorHex { get; set; }

        public string Icon { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}