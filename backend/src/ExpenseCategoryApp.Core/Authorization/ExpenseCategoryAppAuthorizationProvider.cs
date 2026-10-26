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

            // ExpenseItem
            pages.CreateChildPermission(PermissionNames.ExpenseItem_Read, L("ExpenseItem.Read"));
            pages.CreateChildPermission(PermissionNames.ExpenseItem_Create, L("ExpenseItem.Create"));
            pages.CreateChildPermission(PermissionNames.ExpenseItem_Update, L("ExpenseItem.Update"));
            pages.CreateChildPermission(PermissionNames.ExpenseItem_Delete, L("ExpenseItem.Delete"));

            // Budget
            pages.CreateChildPermission(PermissionNames.Budget_Read, L("Budget.Read"));
            pages.CreateChildPermission(PermissionNames.Budget_Create, L("Budget.Create"));
            pages.CreateChildPermission(PermissionNames.Budget_Update, L("Budget.Update"));
            pages.CreateChildPermission(PermissionNames.Budget_Delete, L("Budget.Delete"));

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
