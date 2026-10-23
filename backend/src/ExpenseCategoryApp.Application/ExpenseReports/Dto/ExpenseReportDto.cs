using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseReports.Dto
{
    [AutoMapFrom(typeof(Entities.ExpenseReport))]
    public class ExpenseReportDto : EntityDto<long>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime ReportDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public int Currency { get; set; }

        public int Status { get; set; }

        public string RevisionNote { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long? UserId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}