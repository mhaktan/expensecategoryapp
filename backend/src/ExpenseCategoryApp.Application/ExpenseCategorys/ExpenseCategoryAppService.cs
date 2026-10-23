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
using ExpenseCategoryApp.Approvals.Dto;
using ExpenseCategoryApp.Authorization;
using ExpenseCategoryApp.Flows;

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
        private readonly IFlowEngine _flowEngine;

        public ExpenseCategoryAppService(IRepository<ExpenseCategory, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
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
                    (x.Description != null && x.Description.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
        }

        public override async Task<ExpenseCategoryDto> CreateAsync(CreateExpenseCategoryDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "ExpenseCategory", result);
            return result;
        }

        public override async Task<ExpenseCategoryDto> UpdateAsync(ExpenseCategoryDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "ExpenseCategory", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "ExpenseCategory", new { Id = input.Id });
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
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new ExpenseCategoryReportDto
            {
                Data = ObjectMapper.Map<ExpenseCategoryDto>(root),
                ExpenseItems = ObjectMapper.Map<List<ExpenseItemDto>>(
                    root.ExpenseItems == null ? new List<ExpenseItem>() : root.ExpenseItems.ToList()),
            };
        }

    }
}
