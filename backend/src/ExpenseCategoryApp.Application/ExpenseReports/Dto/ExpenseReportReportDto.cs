using System;
using System.Collections.Generic;
using ExpenseCategoryApp.ExpenseItems.Dto;
using ExpenseCategoryApp.Approvals.Dto;

namespace ExpenseCategoryApp.ExpenseReports.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class ExpenseReportReportDto
    {
        public ExpenseReportDto Data { get; set; }
        public List<ExpenseItemDto> ExpenseItems { get; set; }
        public List<ApprovalRecordDto> ApprovalHistory { get; set; }
    }
}
