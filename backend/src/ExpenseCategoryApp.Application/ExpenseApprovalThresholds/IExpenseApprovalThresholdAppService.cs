using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.ExpenseApprovalThresholds.Dto;

namespace ExpenseCategoryApp.ExpenseApprovalThresholds
{
    public interface IExpenseApprovalThresholdAppService : IAsyncCrudAppService<
        ExpenseApprovalThresholdDto,
        long,
        PagedExpenseApprovalThresholdResultRequestDto,
        CreateExpenseApprovalThresholdDto,
        ExpenseApprovalThresholdDto>
    {
        List<GroupCountDto> GetGroupedCount(ExpenseApprovalThresholdGroupedCountInput input);
        decimal? GetStats(ExpenseApprovalThresholdStatsInput input);
    }
}
