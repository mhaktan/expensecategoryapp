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
using ExpenseCategoryApp.ExpenseApprovalThresholds.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.Authorization;
using ExpenseCategoryApp.Flows;

namespace ExpenseCategoryApp.ExpenseApprovalThresholds
{
    public class ExpenseApprovalThresholdAppService : AsyncCrudAppService<
        ExpenseApprovalThreshold,
        ExpenseApprovalThresholdDto,
        long,
        PagedExpenseApprovalThresholdResultRequestDto,
        CreateExpenseApprovalThresholdDto,
        ExpenseApprovalThresholdDto>,
        IExpenseApprovalThresholdAppService
    {
        private readonly IFlowEngine _flowEngine;

        public ExpenseApprovalThresholdAppService(IRepository<ExpenseApprovalThreshold, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.ExpenseApprovalThreshold_Read;
            GetAllPermissionName = PermissionNames.ExpenseApprovalThreshold_Read;
            CreatePermissionName = PermissionNames.ExpenseApprovalThreshold_Create;
            UpdatePermissionName = PermissionNames.ExpenseApprovalThreshold_Update;
            DeletePermissionName = PermissionNames.ExpenseApprovalThreshold_Delete;
        }

        protected override IQueryable<ExpenseApprovalThreshold> CreateFilteredQuery(PagedExpenseApprovalThresholdResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(input.Currency.HasValue, x => x.Currency == (ExpenseApprovalThresholdCurrency)input.Currency.Value)
                .WhereIf(input.ThresholdAmount.HasValue, x => x.ThresholdAmount == input.ThresholdAmount.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value)
                .WhereIf(input.ThresholdAmountFrom.HasValue, x => x.ThresholdAmount >= input.ThresholdAmountFrom.Value)
                .WhereIf(input.ThresholdAmountTo.HasValue, x => x.ThresholdAmount <= input.ThresholdAmountTo.Value)
                .WhereIf(!input.CurrencyIn.IsNullOrWhiteSpace(), x => input.CurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExpenseApprovalThresholdCurrency?)int.Parse(v.Trim()))
                    .Contains(x.Currency))
                .WhereIf(input.CurrencyNot.HasValue, x => x.Currency != (ExpenseApprovalThresholdCurrency)input.CurrencyNot.Value);
        }

        public override async Task<ExpenseApprovalThresholdDto> CreateAsync(CreateExpenseApprovalThresholdDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "ExpenseApprovalThreshold", result);
            return result;
        }

        public override async Task<ExpenseApprovalThresholdDto> UpdateAsync(ExpenseApprovalThresholdDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "ExpenseApprovalThreshold", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "ExpenseApprovalThreshold", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseApprovalThreshold_Read)]
        public List<GroupCountDto> GetGroupedCount(ExpenseApprovalThresholdGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Currency" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "Currency":
                    return query
                        .GroupBy(x => x.Currency)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseApprovalThreshold_Read)]
        public decimal? GetStats(ExpenseApprovalThresholdStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new string[0];
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "ThresholdAmount" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "ThresholdAmount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.ThresholdAmount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.ThresholdAmount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.ThresholdAmount)
                            : query.Average(x => (decimal?)x.ThresholdAmount);
                        default: return null;
            }
        }

    }
}
