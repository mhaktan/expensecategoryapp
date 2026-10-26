using System.Collections.Generic;
using Abp.Dependency;

namespace ExpenseCategoryApp.Authorization
{
    /// <summary>Single permission descriptor — name, group (entity), description.</summary>
    public class PermissionInfo
    {
        public string Name { get; }
        public string Group { get; }
        public string Description { get; }
        public bool IsRbac { get; }

        public PermissionInfo(string name, string group, string description, bool isRbac)
        {
            Name = name; Group = group; Description = description; IsRbac = isRbac;
        }
    }

    public interface IPermissionRegistry
    {
        IReadOnlyList<PermissionInfo> All { get; }
    }

    public class PermissionRegistry : IPermissionRegistry, ISingletonDependency
    {
        public IReadOnlyList<PermissionInfo> All { get; } = new List<PermissionInfo>
        {
            new PermissionInfo("ExpenseCategory.Read", "ExpenseCategory", "Read ExpenseCategory", false),
            new PermissionInfo("ExpenseCategory.Create", "ExpenseCategory", "Create ExpenseCategory", false),
            new PermissionInfo("ExpenseCategory.Update", "ExpenseCategory", "Update ExpenseCategory", false),
            new PermissionInfo("ExpenseCategory.Delete", "ExpenseCategory", "Delete ExpenseCategory", false),
            new PermissionInfo("ExpenseItem.Read", "ExpenseItem", "Read ExpenseItem", false),
            new PermissionInfo("ExpenseItem.Create", "ExpenseItem", "Create ExpenseItem", false),
            new PermissionInfo("ExpenseItem.Update", "ExpenseItem", "Update ExpenseItem", false),
            new PermissionInfo("ExpenseItem.Delete", "ExpenseItem", "Delete ExpenseItem", false),
            new PermissionInfo("Budget.Read", "Budget", "Read Budget", false),
            new PermissionInfo("Budget.Create", "Budget", "Create Budget", false),
            new PermissionInfo("Budget.Update", "Budget", "Update Budget", false),
            new PermissionInfo("Budget.Delete", "Budget", "Delete Budget", false),
            new PermissionInfo("AppUser.Read", "AppUser", "Read users", true),
            new PermissionInfo("AppRole.Read", "AppRole", "Read roles", true),
            new PermissionInfo("AppUser.Create", "AppUser", "Create users", true),
            new PermissionInfo("AppRole.Create", "AppRole", "Create roles", true),
            new PermissionInfo("AppUser.Update", "AppUser", "Update users", true),
            new PermissionInfo("AppRole.Update", "AppRole", "Update roles", true),
            new PermissionInfo("AppUser.Delete", "AppUser", "Delete users", true),
            new PermissionInfo("AppRole.Delete", "AppRole", "Delete roles", true),
            new PermissionInfo("AppRole.AssignPermissions", "AppRole", "Assign permissions to roles", true),
        };
    }
}
