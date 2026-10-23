using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseApprovalThresholds.Dto
{
    [AutoMapFrom(typeof(Entities.ExpenseApprovalThreshold))]
    public class ExpenseApprovalThresholdDto : EntityDto<long>
    {
        public string Name { get; set; }

        public int Currency { get; set; }

        public decimal ThresholdAmount { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}