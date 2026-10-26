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
using ExpenseCategoryApp.Budgets.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.Authorization;

namespace ExpenseCategoryApp.Budgets
{
    public class BudgetAppService : AsyncCrudAppService<
        Budget,
        BudgetDto,
        long,
        PagedBudgetResultRequestDto,
        CreateBudgetDto,
        BudgetDto>,
        IBudgetAppService
    {
        public BudgetAppService(IRepository<Budget, long> repository)
            : base(repository)
        {
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Budget_Read;
            GetAllPermissionName = PermissionNames.Budget_Read;
            CreatePermissionName = PermissionNames.Budget_Create;
            UpdatePermissionName = PermissionNames.Budget_Update;
            DeletePermissionName = PermissionNames.Budget_Delete;
        }

        protected override IQueryable<Budget> CreateFilteredQuery(PagedBudgetResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(input.LimitAmount.HasValue, x => x.LimitAmount == input.LimitAmount.Value)
                .WhereIf(input.Currency.HasValue, x => x.Currency == (BudgetCurrency)input.Currency.Value)
                .WhereIf(input.PeriodType.HasValue, x => x.PeriodType == (BudgetPeriodType)input.PeriodType.Value)
                .WhereIf(input.StartDate.HasValue, x => x.StartDate == input.StartDate.Value)
                .WhereIf(input.EndDate.HasValue, x => x.EndDate == input.EndDate.Value)
                .WhereIf(input.WarningThresholdPercent.HasValue, x => x.WarningThresholdPercent == input.WarningThresholdPercent.Value)
                .WhereIf(input.LimitAmountFrom.HasValue, x => x.LimitAmount >= input.LimitAmountFrom.Value)
                .WhereIf(input.LimitAmountTo.HasValue, x => x.LimitAmount <= input.LimitAmountTo.Value)
                .WhereIf(input.StartDateFrom.HasValue, x => x.StartDate >= input.StartDateFrom.Value)
                .WhereIf(input.StartDateTo.HasValue, x => x.StartDate <= input.StartDateTo.Value)
                .WhereIf(input.EndDateFrom.HasValue, x => x.EndDate >= input.EndDateFrom.Value)
                .WhereIf(input.EndDateTo.HasValue, x => x.EndDate <= input.EndDateTo.Value)
                .WhereIf(input.WarningThresholdPercentFrom.HasValue, x => x.WarningThresholdPercent >= input.WarningThresholdPercentFrom.Value)
                .WhereIf(input.WarningThresholdPercentTo.HasValue, x => x.WarningThresholdPercent <= input.WarningThresholdPercentTo.Value)
                .WhereIf(!input.CurrencyIn.IsNullOrWhiteSpace(), x => input.CurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (BudgetCurrency?)int.Parse(v.Trim()))
                    .Contains(x.Currency))
                .WhereIf(input.CurrencyNot.HasValue, x => x.Currency != (BudgetCurrency)input.CurrencyNot.Value)
                .WhereIf(!input.PeriodTypeIn.IsNullOrWhiteSpace(), x => input.PeriodTypeIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (BudgetPeriodType?)int.Parse(v.Trim()))
                    .Contains(x.PeriodType))
                .WhereIf(input.PeriodTypeNot.HasValue, x => x.PeriodType != (BudgetPeriodType)input.PeriodTypeNot.Value)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.ExpenseCategoryId.HasValue, x => x.ExpenseCategoryId == input.ExpenseCategoryId.Value);
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Budget_Read)]
        public List<GroupCountDto> GetGroupedCount(BudgetGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Currency", "PeriodType", "UserId", "ExpenseCategoryId" };
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
                case "PeriodType":
                    return query
                        .GroupBy(x => x.PeriodType)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "UserId":
                    return query
                        .GroupBy(x => new { Key = x.UserId, Label = x.User == null ? null : x.User.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "ExpenseCategoryId":
                    return query
                        .GroupBy(x => new { Key = x.ExpenseCategoryId, Label = x.ExpenseCategory == null ? null : x.ExpenseCategory.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.Budget_Read)]
        public decimal? GetStats(BudgetStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "StartDate", "EndDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "StartDate|EndDate":
                    {
                        var pairsStartDateEndDate = query
                            .Where(x => x.EndDate != null)
                            .Select(x => new { A = x.StartDate, B = x.EndDate.Value })
                            .ToList();
                        if (pairsStartDateEndDate.Count == 0) return null;
                        return (decimal)pairsStartDateEndDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "EndDate|StartDate":
                    {
                        var pairsEndDateStartDate = query
                            .Where(x => x.EndDate != null)
                            .Select(x => new { A = x.EndDate.Value, B = x.StartDate })
                            .ToList();
                        if (pairsEndDateStartDate.Count == 0) return null;
                        return (decimal)pairsEndDateStartDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "LimitAmount", "WarningThresholdPercent" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "LimitAmount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.LimitAmount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.LimitAmount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.LimitAmount)
                            : query.Average(x => (decimal?)x.LimitAmount);
                        case "WarningThresholdPercent": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.WarningThresholdPercent)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.WarningThresholdPercent)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.WarningThresholdPercent)
                            : query.Average(x => (decimal?)x.WarningThresholdPercent);
                        default: return null;
            }
        }

    }
}
