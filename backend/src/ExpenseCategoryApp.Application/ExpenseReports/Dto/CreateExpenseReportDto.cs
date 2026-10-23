using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace ExpenseCategoryApp.ExpenseReports.Dto
{
    [AutoMapTo(typeof(Entities.ExpenseReport))]
    public class CreateExpenseReportDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public DateTime ReportDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public int Currency { get; set; }

        public int Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long? UserId { get; set; }

    }
}