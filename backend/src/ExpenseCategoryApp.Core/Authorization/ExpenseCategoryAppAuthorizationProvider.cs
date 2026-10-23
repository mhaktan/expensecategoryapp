using Abp.Authorization;
using Abp.Localization;

namespace ExpenseCategoryApp.Authorization
{
    public class ExpenseCategoryAppAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull("Pages") ?? context.CreatePermission("Pages", L("Pages"));

            // ExpenseCategory
            pages.CreateChildPermission(PermissionNames.ExpenseCategory_Read, L("ExpenseCategory.Read"));
            pages.CreateChildPermission(PermissionNames.ExpenseCategory_Create, L("ExpenseCategory.Create"));
            pages.CreateChildPermission(PermissionNames.ExpenseCategory_Update, L("ExpenseCategory.Update"));
            pages.CreateChildPermission(PermissionNames.ExpenseCategory_Delete, L("ExpenseCategory.Delete"));

            // ExpenseApprovalThreshold
            pages.CreateChildPermission(PermissionNames.ExpenseApprovalThreshold_Read, L("ExpenseApprovalThreshold.Read"));
            pages.CreateChildPermission(PermissionNames.ExpenseApprovalThreshold_Create, L("ExpenseApprovalThreshold.Create"));
            pages.CreateChildPermission(PermissionNames.ExpenseApprovalThreshold_Update, L("ExpenseApprovalThreshold.Update"));
            pages.CreateChildPermission(PermissionNames.ExpenseApprovalThreshold_Delete, L("ExpenseApprovalThreshold.Delete"));

            // ExpenseReport
            pages.CreateChildPermission(PermissionNames.ExpenseReport_Read, L("ExpenseReport.Read"));
            pages.CreateChildPermission(PermissionNames.ExpenseReport_Create, L("ExpenseReport.Create"));
            pages.CreateChildPermission(PermissionNames.ExpenseReport_Update, L("ExpenseReport.Update"));
            pages.CreateChildPermission(PermissionNames.ExpenseReport_Delete, L("ExpenseReport.Delete"));
            pages.CreateChildPermission(PermissionNames.ExpenseReport_ChangeStatus, L("ExpenseReport.ChangeStatus"));

            // ExpenseItem
            pages.CreateChildPermission(PermissionNames.ExpenseItem_Read, L("ExpenseItem.Read"));
            pages.CreateChildPermission(PermissionNames.ExpenseItem_Create, L("ExpenseItem.Create"));
            pages.CreateChildPermission(PermissionNames.ExpenseItem_Update, L("ExpenseItem.Update"));
            pages.CreateChildPermission(PermissionNames.ExpenseItem_Delete, L("ExpenseItem.Delete"));

            // RBAC
            pages.CreateChildPermission(PermissionNames.AppUser_Read, L("AppUser.Read"));
            pages.CreateChildPermission(PermissionNames.AppRole_Read, L("AppRole.Read"));
            pages.CreateChildPermission(PermissionNames.AppUser_Create, L("AppUser.Create"));
            pages.CreateChildPermission(PermissionNames.AppRole_Create, L("AppRole.Create"));
            pages.CreateChildPermission(PermissionNames.AppUser_Update, L("AppUser.Update"));
            pages.CreateChildPermission(PermissionNames.AppRole_Update, L("AppRole.Update"));
            pages.CreateChildPermission(PermissionNames.AppUser_Delete, L("AppUser.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_Delete, L("AppRole.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_AssignPermissions, L("AppRole.AssignPermissions"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ExpenseCategoryAppConsts.LocalizationSourceName);
        }
    }
}
