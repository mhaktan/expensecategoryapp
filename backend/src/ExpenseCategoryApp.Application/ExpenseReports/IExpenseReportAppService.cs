using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.StateMachine.Dto;
using ExpenseCategoryApp.ExpenseReports.Dto;

namespace ExpenseCategoryApp.ExpenseReports
{
    public interface IExpenseReportAppService : IAsyncCrudAppService<
        ExpenseReportDto,
        long,
        PagedExpenseReportResultRequestDto,
        CreateExpenseReportDto,
        ExpenseReportDto>
    {
        Task<ExpenseReportDto> ChangeStatusAsync(long id, ChangeStatusInput input);

        // Yalniz ApprovalAppService cagirir — REST'e acik degil
        [Abp.Application.Services.RemoteService(false)]
        Task<ExpenseReportDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(ExpenseReportGroupedCountInput input);
        decimal? GetStats(ExpenseReportStatsInput input);
        Task<ExpenseReportReportDto> GetReportData(long id);
    }
}
