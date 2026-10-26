using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.Budgets.Dto
{
    [AutoMapFrom(typeof(Entities.Budget))]
    public class BudgetDto : EntityDto<long>
    {
        public string Name { get; set; }

        public decimal LimitAmount { get; set; }

        public int Currency { get; set; }

        public int PeriodType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? WarningThresholdPercent { get; set; }

        public long? UserId { get; set; }

        public long ExpenseCategoryId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}