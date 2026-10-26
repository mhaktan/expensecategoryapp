using System;
using System.Collections.Generic;
using ExpenseCategoryApp.ExpenseItems.Dto;
using ExpenseCategoryApp.Budgets.Dto;

namespace ExpenseCategoryApp.ExpenseCategorys.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class ExpenseCategoryReportDto
    {
        public ExpenseCategoryDto Data { get; set; }
        public List<ExpenseItemDto> ExpenseItems { get; set; }
        public List<BudgetDto> Budgets { get; set; }
    }
}
