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

        // ExpenseItem
        public const string ExpenseItem_Read = "ExpenseItem.Read";
        public const string ExpenseItem_Create = "ExpenseItem.Create";
        public const string ExpenseItem_Update = "ExpenseItem.Update";
        public const string ExpenseItem_Delete = "ExpenseItem.Delete";

        // Budget
        public const string Budget_Read = "Budget.Read";
        public const string Budget_Create = "Budget.Create";
        public const string Budget_Update = "Budget.Update";
        public const string Budget_Delete = "Budget.Delete";

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
