using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.ExpenseItems.Dto;

namespace ExpenseCategoryApp.ExpenseItems
{
    public interface IExpenseItemAppService : IAsyncCrudAppService<
        ExpenseItemDto,
        long,
        PagedExpenseItemResultRequestDto,
        CreateExpenseItemDto,
        ExpenseItemDto>
    {
        List<GroupCountDto> GetGroupedCount(ExpenseItemGroupedCountInput input);
        decimal? GetStats(ExpenseItemStatsInput input);
    }
}
