using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.ExpenseCategorys.Dto;

namespace ExpenseCategoryApp.ExpenseCategorys
{
    public interface IExpenseCategoryAppService : IAsyncCrudAppService<
        ExpenseCategoryDto,
        long,
        PagedExpenseCategoryResultRequestDto,
        CreateExpenseCategoryDto,
        ExpenseCategoryDto>
    {
        Task<ExpenseCategoryReportDto> GetReportData(long id);
    }
}
