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
using ExpenseCategoryApp.ExpenseItems.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.Authorization;
using ExpenseCategoryApp.Flows;

namespace ExpenseCategoryApp.ExpenseItems
{
    public class ExpenseItemAppService : AsyncCrudAppService<
        ExpenseItem,
        ExpenseItemDto,
        long,
        PagedExpenseItemResultRequestDto,
        CreateExpenseItemDto,
        ExpenseItemDto>,
        IExpenseItemAppService
    {
        private readonly IFlowEngine _flowEngine;

        public ExpenseItemAppService(IRepository<ExpenseItem, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.ExpenseItem_Read;
            GetAllPermissionName = PermissionNames.ExpenseItem_Read;
            CreatePermissionName = PermissionNames.ExpenseItem_Create;
            UpdatePermissionName = PermissionNames.ExpenseItem_Update;
            DeletePermissionName = PermissionNames.ExpenseItem_Delete;
        }

        protected override IQueryable<ExpenseItem> CreateFilteredQuery(PagedExpenseItemResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.ReceiptNo != null && x.ReceiptNo.Contains(input.Keyword)))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.ReceiptNo.IsNullOrWhiteSpace(), x => x.ReceiptNo != null && x.ReceiptNo.Contains(input.ReceiptNo))
                .WhereIf(input.ExpenseDate.HasValue, x => x.ExpenseDate == input.ExpenseDate.Value)
                .WhereIf(input.Amount.HasValue, x => x.Amount == input.Amount.Value)
                .WhereIf(input.Currency.HasValue, x => x.Currency == (ExpenseItemCurrency)input.Currency.Value)
                .WhereIf(input.ExchangeRate.HasValue, x => x.ExchangeRate == input.ExchangeRate.Value)
                .WhereIf(input.AmountInTRY.HasValue, x => x.AmountInTRY == input.AmountInTRY.Value)
                .WhereIf(input.ExpenseDateFrom.HasValue, x => x.ExpenseDate >= input.ExpenseDateFrom.Value)
                .WhereIf(input.ExpenseDateTo.HasValue, x => x.ExpenseDate <= input.ExpenseDateTo.Value)
                .WhereIf(input.AmountFrom.HasValue, x => x.Amount >= input.AmountFrom.Value)
                .WhereIf(input.AmountTo.HasValue, x => x.Amount <= input.AmountTo.Value)
                .WhereIf(input.ExchangeRateFrom.HasValue, x => x.ExchangeRate >= input.ExchangeRateFrom.Value)
                .WhereIf(input.ExchangeRateTo.HasValue, x => x.ExchangeRate <= input.ExchangeRateTo.Value)
                .WhereIf(input.AmountInTRYFrom.HasValue, x => x.AmountInTRY >= input.AmountInTRYFrom.Value)
                .WhereIf(input.AmountInTRYTo.HasValue, x => x.AmountInTRY <= input.AmountInTRYTo.Value)
                .WhereIf(!input.CurrencyIn.IsNullOrWhiteSpace(), x => input.CurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExpenseItemCurrency?)int.Parse(v.Trim()))
                    .Contains(x.Currency))
                .WhereIf(input.CurrencyNot.HasValue, x => x.Currency != (ExpenseItemCurrency)input.CurrencyNot.Value)
                .WhereIf(input.ExpenseReportId.HasValue, x => x.ExpenseReportId == input.ExpenseReportId.Value)
                .WhereIf(input.ExpenseCategoryId.HasValue, x => x.ExpenseCategoryId == input.ExpenseCategoryId.Value);
        }

        public override async Task<ExpenseItemDto> CreateAsync(CreateExpenseItemDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "ExpenseItem", result);
            return result;
        }

        public override async Task<ExpenseItemDto> UpdateAsync(ExpenseItemDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "ExpenseItem", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "ExpenseItem", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseItem_Read)]
        public List<GroupCountDto> GetGroupedCount(ExpenseItemGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Currency", "ExpenseReportId", "ExpenseCategoryId" };
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
                case "ExpenseReportId":
                    return query
                        .GroupBy(x => new { Key = x.ExpenseReportId, Label = x.ExpenseReport == null ? null : x.ExpenseReport.Title })
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseItem_Read)]
        public decimal? GetStats(ExpenseItemStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "ExpenseDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Amount", "ExchangeRate", "AmountInTRY" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Amount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Amount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Amount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Amount)
                            : query.Average(x => (decimal?)x.Amount);
                        case "ExchangeRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.ExchangeRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.ExchangeRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.ExchangeRate)
                            : query.Average(x => (decimal?)x.ExchangeRate);
                        case "AmountInTRY": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.AmountInTRY)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.AmountInTRY)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.AmountInTRY)
                            : query.Average(x => (decimal?)x.AmountInTRY);
                        default: return null;
            }
        }

    }
}
