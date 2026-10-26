using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using ExpenseCategoryApp.Entities;
using ExpenseCategoryApp.ExpenseCategorys.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.ExpenseItems.Dto;
using ExpenseCategoryApp.Budgets.Dto;
using ExpenseCategoryApp.Authorization;

namespace ExpenseCategoryApp.ExpenseCategorys
{
    public class ExpenseCategoryAppService : AsyncCrudAppService<
        ExpenseCategory,
        ExpenseCategoryDto,
        long,
        PagedExpenseCategoryResultRequestDto,
        CreateExpenseCategoryDto,
        ExpenseCategoryDto>,
        IExpenseCategoryAppService
    {
        public ExpenseCategoryAppService(IRepository<ExpenseCategory, long> repository)
            : base(repository)
        {
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.ExpenseCategory_Read;
            GetAllPermissionName = PermissionNames.ExpenseCategory_Read;
            CreatePermissionName = PermissionNames.ExpenseCategory_Create;
            UpdatePermissionName = PermissionNames.ExpenseCategory_Update;
            DeletePermissionName = PermissionNames.ExpenseCategory_Delete;
        }

        protected override IQueryable<ExpenseCategory> CreateFilteredQuery(PagedExpenseCategoryResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.ColorHex != null && x.ColorHex.Contains(input.Keyword)) ||
                    (x.Icon != null && x.Icon.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.ColorHex.IsNullOrWhiteSpace(), x => x.ColorHex != null && x.ColorHex.Contains(input.ColorHex))
                .WhereIf(!input.Icon.IsNullOrWhiteSpace(), x => x.Icon != null && x.Icon.Contains(input.Icon));
        }
        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseCategory_Read)]
        public async Task<ExpenseCategoryReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.ExpenseItems)
                .Include(x => x.Budgets)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new ExpenseCategoryReportDto
            {
                Data = ObjectMapper.Map<ExpenseCategoryDto>(root),
                ExpenseItems = ObjectMapper.Map<List<ExpenseItemDto>>(
                    root.ExpenseItems == null ? new List<ExpenseItem>() : root.ExpenseItems.ToList()),
                Budgets = ObjectMapper.Map<List<BudgetDto>>(
                    root.Budgets == null ? new List<Budget>() : root.Budgets.ToList()),
            };
        }

    }
}
