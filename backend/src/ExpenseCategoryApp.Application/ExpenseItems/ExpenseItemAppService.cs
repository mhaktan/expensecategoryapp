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
        public ExpenseItemAppService(IRepository<ExpenseItem, long> repository)
            : base(repository)
        {
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
                    (x.Title != null && x.Title.Contains(input.Keyword)) ||
                    (x.Notes != null && x.Notes.Contains(input.Keyword)))
                .WhereIf(!input.Title.IsNullOrWhiteSpace(), x => x.Title != null && x.Title.Contains(input.Title))
                .WhereIf(!input.Notes.IsNullOrWhiteSpace(), x => x.Notes != null && x.Notes.Contains(input.Notes))
                .WhereIf(input.Amount.HasValue, x => x.Amount == input.Amount.Value)
                .WhereIf(input.Currency.HasValue, x => x.Currency == (ExpenseItemCurrency)input.Currency.Value)
                .WhereIf(input.ExpenseDate.HasValue, x => x.ExpenseDate == input.ExpenseDate.Value)
                .WhereIf(input.PaymentMethod.HasValue, x => x.PaymentMethod == (ExpenseItemPaymentMethod)input.PaymentMethod.Value)
                .WhereIf(input.IsRecurring.HasValue, x => x.IsRecurring == input.IsRecurring.Value)
                .WhereIf(input.AmountFrom.HasValue, x => x.Amount >= input.AmountFrom.Value)
                .WhereIf(input.AmountTo.HasValue, x => x.Amount <= input.AmountTo.Value)
                .WhereIf(input.ExpenseDateFrom.HasValue, x => x.ExpenseDate >= input.ExpenseDateFrom.Value)
                .WhereIf(input.ExpenseDateTo.HasValue, x => x.ExpenseDate <= input.ExpenseDateTo.Value)
                .WhereIf(!input.CurrencyIn.IsNullOrWhiteSpace(), x => input.CurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExpenseItemCurrency?)int.Parse(v.Trim()))
                    .Contains(x.Currency))
                .WhereIf(input.CurrencyNot.HasValue, x => x.Currency != (ExpenseItemCurrency)input.CurrencyNot.Value)
                .WhereIf(!input.PaymentMethodIn.IsNullOrWhiteSpace(), x => input.PaymentMethodIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExpenseItemPaymentMethod?)int.Parse(v.Trim()))
                    .Contains(x.PaymentMethod))
                .WhereIf(input.PaymentMethodNot.HasValue, x => x.PaymentMethod != (ExpenseItemPaymentMethod)input.PaymentMethodNot.Value)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.ExpenseCategoryId.HasValue, x => x.ExpenseCategoryId == input.ExpenseCategoryId.Value);
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseItem_Read)]
        public List<GroupCountDto> GetGroupedCount(ExpenseItemGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Currency", "PaymentMethod", "UserId", "ExpenseCategoryId" };
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
                case "PaymentMethod":
                    return query
                        .GroupBy(x => x.PaymentMethod)
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

            var allowedNumeric = new[] { "Amount" };
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
                        default: return null;
            }
        }

    }
}
