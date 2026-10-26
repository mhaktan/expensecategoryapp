using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.Budgets.Dto;

namespace ExpenseCategoryApp.Budgets
{
    public interface IBudgetAppService : IAsyncCrudAppService<
        BudgetDto,
        long,
        PagedBudgetResultRequestDto,
        CreateBudgetDto,
        BudgetDto>
    {
        List<GroupCountDto> GetGroupedCount(BudgetGroupedCountInput input);
        decimal? GetStats(BudgetStatsInput input);
    }
}
