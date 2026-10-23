namespace ExpenseCategoryApp.Authorization
{
    public static class PermissionNames
    {
        public const string Pages = "Pages";

        // ExpenseCategory
        public const string ExpenseCategory_Read = "ExpenseCategory.Read";
        public const string ExpenseCategory_Create = "ExpenseCategory.Create";
        public const string ExpenseCategory_Update = "ExpenseCategory.Update";
        public const string ExpenseCategory_Delete = "ExpenseCategory.Delete";

        // ExpenseApprovalThreshold
        public const string ExpenseApprovalThreshold_Read = "ExpenseApprovalThreshold.Read";
        public const string ExpenseApprovalThreshold_Create = "ExpenseApprovalThreshold.Create";
        public const string ExpenseApprovalThreshold_Update = "ExpenseApprovalThreshold.Update";
        public const string ExpenseApprovalThreshold_Delete = "ExpenseApprovalThreshold.Delete";

        // ExpenseReport
        public const string ExpenseReport_Read = "ExpenseReport.Read";
        public const string ExpenseReport_Create = "ExpenseReport.Create";
        public const string ExpenseReport_Update = "ExpenseReport.Update";
        public const string ExpenseReport_Delete = "ExpenseReport.Delete";
        public const string ExpenseReport_ChangeStatus = "ExpenseReport.ChangeStatus";

        // ExpenseItem
        public const string ExpenseItem_Read = "ExpenseItem.Read";
        public const string ExpenseItem_Create = "ExpenseItem.Create";
        public const string ExpenseItem_Update = "ExpenseItem.Update";
        public const string ExpenseItem_Delete = "ExpenseItem.Delete";

        // RBAC management
        public const string AppUser_Read = "AppUser.Read";
        public const string AppRole_Read = "AppRole.Read";
        public const string AppUser_Create = "AppUser.Create";
        public const string AppRole_Create = "AppRole.Create";
        public const string AppUser_Update = "AppUser.Update";
        public const string AppRole_Update = "AppRole.Update";
        public const string AppUser_Delete = "AppUser.Delete";
        public const string AppRole_Delete = "AppRole.Delete";
        public const string AppRole_AssignPermissions = "AppRole.AssignPermissions";

    }
}
