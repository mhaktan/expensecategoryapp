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
using ExpenseCategoryApp.ExpenseReports.Dto;
using ExpenseCategoryApp.Analytics.Dto;
using ExpenseCategoryApp.StateMachine.Dto;
using ExpenseCategoryApp.ExpenseItems.Dto;
using ExpenseCategoryApp.Approvals.Dto;
using ExpenseCategoryApp.Authorization;
using ExpenseCategoryApp.Flows;

namespace ExpenseCategoryApp.ExpenseReports
{
    public class ExpenseReportAppService : AsyncCrudAppService<
        ExpenseReport,
        ExpenseReportDto,
        long,
        PagedExpenseReportResultRequestDto,
        CreateExpenseReportDto,
        ExpenseReportDto>,
        IExpenseReportAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public ExpenseReportAppService(IRepository<ExpenseReport, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.ExpenseReport_Read;
            GetAllPermissionName = PermissionNames.ExpenseReport_Read;
            CreatePermissionName = PermissionNames.ExpenseReport_Create;
            UpdatePermissionName = PermissionNames.ExpenseReport_Update;
            DeletePermissionName = PermissionNames.ExpenseReport_Delete;
        }

        protected override IQueryable<ExpenseReport> CreateFilteredQuery(PagedExpenseReportResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Title != null && x.Title.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.RevisionNote != null && x.RevisionNote.Contains(input.Keyword)))
                .WhereIf(!input.Title.IsNullOrWhiteSpace(), x => x.Title != null && x.Title.Contains(input.Title))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.RevisionNote.IsNullOrWhiteSpace(), x => x.RevisionNote != null && x.RevisionNote.Contains(input.RevisionNote))
                .WhereIf(input.ReportDate.HasValue, x => x.ReportDate == input.ReportDate.Value)
                .WhereIf(input.TotalAmount.HasValue, x => x.TotalAmount == input.TotalAmount.Value)
                .WhereIf(input.Currency.HasValue, x => x.Currency == (ExpenseReportCurrency)input.Currency.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (ExpenseReportStatus)input.Status.Value)
                .WhereIf(input.ReportDateFrom.HasValue, x => x.ReportDate >= input.ReportDateFrom.Value)
                .WhereIf(input.ReportDateTo.HasValue, x => x.ReportDate <= input.ReportDateTo.Value)
                .WhereIf(input.TotalAmountFrom.HasValue, x => x.TotalAmount >= input.TotalAmountFrom.Value)
                .WhereIf(input.TotalAmountTo.HasValue, x => x.TotalAmount <= input.TotalAmountTo.Value)
                .WhereIf(!input.CurrencyIn.IsNullOrWhiteSpace(), x => input.CurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExpenseReportCurrency?)int.Parse(v.Trim()))
                    .Contains(x.Currency))
                .WhereIf(input.CurrencyNot.HasValue, x => x.Currency != (ExpenseReportCurrency)input.CurrencyNot.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ExpenseReportStatus?)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (ExpenseReportStatus)input.StatusNot.Value)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value);
        }

        public override async Task<ExpenseReportDto> CreateAsync(CreateExpenseReportDto input)
        {
            // Baslangic disi bir durumla olusturma yalniz tek bir gecerli gecisle olabilir
            // (onay adimlarinin "Approve" gecisleri haric) — aksi halde onay atlanirdi.
            CheckCreatePermission();
            if (input.Status != (int)ExpenseReportStatus.Draft)
                ValidateStatusTransition(ExpenseReportStatus.Draft, (ExpenseReportStatus)input.Status);
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "ExpenseReport", result);

            // Frontend creates records with status pre-set without going through ChangeStatusAsync,
            // so mirror on-field-change here whenever the initial status isn't the default. Otherwise
            // status-driven flows (e.g. approval) never fire on plain Create.
            if (result.Status != (int)ExpenseReportStatus.Draft)
                await _flowEngine.TriggerAsync("on-field-change", "ExpenseReport", result);
            return result;
        }

        public override async Task<ExpenseReportDto> UpdateAsync(ExpenseReportDto input)
        {
            // State machine: validate status transition + log
            var existing = await Repository.GetAsync(input.Id);
            var statusChanged = (int)existing.Status != input.Status;
            if (statusChanged)
            {
                var fromStatus = existing.Status.ToString();
                var toStatus = ((ExpenseReportStatus)input.Status).ToString();
                ValidateStatusTransition(existing.Status, (ExpenseReportStatus)input.Status);

                // Log status change
                await _statusChangeLogRepo.InsertAsync(new StatusChangeLog
                {
                    EntityType = "ExpenseReport",
                    EntityId = input.Id.ToString(),
                    FromStatus = fromStatus,
                    ToStatus = toStatus,
                    Action = "Update",
                    ChangedByUserId = AbpSession.UserId
                });
            }

            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "ExpenseReport", result);

            // Frontend updates status via plain UpdateAsync (not ChangeStatusAsync) — fire
            // on-field-change so status-driven flows pick up the transition.
            if (statusChanged)
                await _flowEngine.TriggerAsync("on-field-change", "ExpenseReport", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "ExpenseReport", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseReport_ChangeStatus, PermissionNames.ExpenseReport_Update, RequireAllPermissions = false)]
        public async Task<ExpenseReportDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            // Onay adimlari yalniz atanmis onayci tarafindan, onay servisi uzerinden ilerletilir
            if (input.Action == "Approve")
                throw new Abp.UI.UserFriendlyException("Onay adımları yalnızca onay ekranından ilerletilebilir.");
            return await ApplyStatusChangeAsync(id, input);
        }

        // ABP dinamik API'si attribute'u implementasyonda arar (arayuzdeki yetmez)
        [Abp.Application.Services.RemoteService(false)]
        public async Task<ExpenseReportDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input)
            => await ApplyStatusChangeAsync(id, input);

        private async Task<ExpenseReportDto> ApplyStatusChangeAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Draft", "PendingManagerApproval", "Submit", false),
            ("PendingManagerApproval", "PendingFinanceApproval", "Approve", false),
            ("PendingManagerApproval", "Revision", "Revise", false),
            ("PendingFinanceApproval", "Approved", "Approve", true),
            ("PendingFinanceApproval", "Revision", "Revise", false),
            ("Revision", "PendingManagerApproval", "Resubmit", false),
            ("PendingManagerApproval", "Rejected", "Reject", true),
            ("PendingFinanceApproval", "Rejected", "Reject", true),
            ("*", "Cancelled", "Cancel", false)
            };

            var transition = transitions.FirstOrDefault(t =>
                (t.From == "*" || t.From == currentStatus) && t.Action == input.Action);

            if (transition == default)
                throw new Abp.UI.UserFriendlyException($"Invalid action '{input.Action}' from status '{currentStatus}'");

            // Onay ekranindan gelen serbest metni gecisin zorunlu alanlarina tasi
            if (input.ActionData == null) input.ActionData = new Dictionary<string, string>();
            var genericNote = input.ActionData.ContainsKey("comment") && !string.IsNullOrWhiteSpace(input.ActionData["comment"])
                ? input.ActionData["comment"]
                : (input.ActionData.ContainsKey("revisionNote") ? input.ActionData["revisionNote"] : null);
            if (!string.IsNullOrWhiteSpace(genericNote))
            {
                foreach (var rf in new[] { "revisionNote" })
                {
                    if (!input.ActionData.ContainsKey(rf) || string.IsNullOrWhiteSpace(input.ActionData[rf]))
                        input.ActionData[rf] = genericNote;
                }
            }
            // Validate required fields per transition
            if (input.Action == "Revise" && (input.ActionData == null || !input.ActionData.ContainsKey("revisionNote") || string.IsNullOrWhiteSpace(input.ActionData["revisionNote"])))
                throw new Abp.UI.UserFriendlyException("Revise requires: revisionNote");
            if (input.Action == "Reject" && (input.ActionData == null || !input.ActionData.ContainsKey("revisionNote") || string.IsNullOrWhiteSpace(input.ActionData["revisionNote"])))
                throw new Abp.UI.UserFriendlyException("Reject requires: revisionNote");
            // Bu gecisler icin bagli kayit on kosulu yok

            var fromStatus = currentStatus;

            // Apply new status
            entity.Status = (ExpenseReportStatus)Enum.Parse(typeof(ExpenseReportStatus), transition.To);
            if (input.ActionData != null && input.ActionData.ContainsKey("revisionNote") && !string.IsNullOrWhiteSpace(input.ActionData["revisionNote"]))
                entity.RevisionNote = input.ActionData["revisionNote"];

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "ExpenseReport" && a.EntityId == id.ToString() && a.Status == "Pending")
                    .ToList();
                foreach (var pendingRec in pending)
                {
                    pendingRec.Status = "Cancelled";
                    pendingRec.ActionTaken = "Cancel";
                    pendingRec.ActionDate = DateTime.UtcNow;
                    pendingRec.Comment = "Entity cancelled by submitter.";
                    await _approvalRepo.UpdateAsync(pendingRec);
                }
            }

            // Log status change
            await _statusChangeLogRepo.InsertAsync(new Entities.StatusChangeLog
            {
                EntityType = "ExpenseReport",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "ExpenseReport", result);

            // Trigger named flow events
            if (input.Action == "Submit")
                await _flowEngine.TriggerAsync("submit-for-approval", "ExpenseReport", result);
            return result;
        }

        private void ValidateStatusTransition(ExpenseReportStatus from, ExpenseReportStatus to)
        {
            var allowed = new (string From, string To)[]
            {
                ("Draft", "PendingManagerApproval"),
                ("PendingManagerApproval", "Revision"),
                ("PendingFinanceApproval", "Revision"),
                ("Revision", "PendingManagerApproval"),
                ("PendingManagerApproval", "Rejected"),
                ("PendingFinanceApproval", "Rejected"),
                ("*", "Cancelled")
            };

            var isValid = allowed.Any(t =>
                (t.From == "*" || t.From == from.ToString()) &&
                t.To == to.ToString());

            if (!isValid)
                throw new Abp.UI.UserFriendlyException($"Invalid status transition from {from} to {to}");
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseReport_Read)]
        public List<GroupCountDto> GetGroupedCount(ExpenseReportGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Currency", "Status", "UserId" };
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
                case "Status":
                    return query
                        .GroupBy(x => x.Status)
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
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseReport_Read)]
        public decimal? GetStats(ExpenseReportStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "ReportDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "TotalAmount" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "TotalAmount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.TotalAmount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.TotalAmount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.TotalAmount)
                            : query.Average(x => (decimal?)x.TotalAmount);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.ExpenseReport_Read)]
        public async Task<ExpenseReportReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.ExpenseItems)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new ExpenseReportReportDto
            {
                Data = ObjectMapper.Map<ExpenseReportDto>(root),
                ExpenseItems = ObjectMapper.Map<List<ExpenseItemDto>>(
                    root.ExpenseItems == null ? new List<ExpenseItem>() : root.ExpenseItems.ToList()),
                // ApprovalRecord alan adlari: EntityType (isim) ve EntityId (STRING).
                // Once EntityName/long varsayilmisti — CS1061 + CS0019 veriyordu.
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityType == "ExpenseReport" && a.EntityId == id.ToString())
                        .OrderBy(a => a.StepIndex).ToList()),
            };
        }

    }
}
