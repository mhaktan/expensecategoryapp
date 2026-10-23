using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using ExpenseCategoryApp.Authorization;
using ExpenseCategoryApp.Entities;

namespace ExpenseCategoryApp.EntityFrameworkCore.Seed
{
    /// <summary>
    /// Seeds default RBAC data on first run:
    ///   - Admin role (IsSystem) granted ALL permissions from PermissionRegistry
    ///   - User role (IsSystem) granted only *.Read permissions
    ///   - admin/123qwe user assigned to Admin role
    /// Idempotent — safe to run on every startup.
    /// </summary>
    public class DefaultUserSeeder
    {
        private readonly IRepository<AppUser, long> _userRepo;
        private readonly IRepository<AppRole, long> _roleRepo;
        private readonly IRepository<UserRole, long> _userRoleRepo;
        private readonly IRepository<RolePermission, long> _rolePermRepo;
        private readonly IPermissionRegistry _permRegistry;

        public DefaultUserSeeder(
            IRepository<AppUser, long> userRepo,
            IRepository<AppRole, long> roleRepo,
            IRepository<UserRole, long> userRoleRepo,
            IRepository<RolePermission, long> rolePermRepo,
            IPermissionRegistry permRegistry)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _userRoleRepo = userRoleRepo;
            _rolePermRepo = rolePermRepo;
            _permRegistry = permRegistry;
        }

        public void Seed()
        {
            // Each step is logged so silent failures at any stage are immediately visible.
            try
            {
                Console.WriteLine("[Seed] Step 1/4: Admin role");
                var adminRole = _roleRepo.GetAll().FirstOrDefault(r => r.Name == "Admin");
                if (adminRole == null)
                {
                    adminRole = new AppRole { Name = "Admin", DisplayName = "Administrator", Description = "Full system access", IsSystem = true, IsActive = true };
                    adminRole.Id = _roleRepo.InsertAndGetId(adminRole);
                    Console.WriteLine($"[Seed] Created Admin role (id={adminRole.Id})");
                }

                Console.WriteLine("[Seed] Step 2/4: Grant all permissions to Admin");
                var allPermNames = _permRegistry.All.Select(p => p.Name).ToList();
                var adminGranted = _rolePermRepo.GetAll().Where(p => p.RoleId == adminRole.Id).Select(p => p.PermissionName).ToList();
                int granted = 0;
                foreach (var pn in allPermNames.Except(adminGranted))
                {
                    _rolePermRepo.Insert(new RolePermission { RoleId = adminRole.Id, PermissionName = pn });
                    granted++;
                }
                Console.WriteLine($"[Seed] Granted {granted} new permissions to Admin (total: {allPermNames.Count})");

                Console.WriteLine("[Seed] Step 3/4: User role + read-only permissions");
                var userRole = _roleRepo.GetAll().FirstOrDefault(r => r.Name == "User");
                if (userRole == null)
                {
                    userRole = new AppRole { Name = "User", DisplayName = "Standard User", Description = "Read-only access to business data", IsSystem = true, IsActive = true };
                    userRole.Id = _roleRepo.InsertAndGetId(userRole);
                    Console.WriteLine($"[Seed] Created User role (id={userRole.Id})");
                }
                var readOnlyPerms = _permRegistry.All.Where(p => !p.IsRbac && p.Name.EndsWith(".Read")).Select(p => p.Name).ToList();
                var userGranted = _rolePermRepo.GetAll().Where(p => p.RoleId == userRole.Id).Select(p => p.PermissionName).ToList();
                foreach (var pn in readOnlyPerms.Except(userGranted))
                    _rolePermRepo.Insert(new RolePermission { RoleId = userRole.Id, PermissionName = pn });

                // Canli ortam (Go Live) ADMIN_PASSWORD'u rastgele set eder; yoksa yalniz yerel gelistirme varsayilani
                var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
                if (string.IsNullOrWhiteSpace(adminPassword))
                {
                    adminPassword = "123qwe";
                    Console.WriteLine("[Seed] UYARI: ADMIN_PASSWORD tanimli degil — varsayilan admin/123qwe kullaniliyor. Canli ortamda mutlaka ayarlayin.");
                }
                Console.WriteLine("[Seed] Step 4/4: Admin user");
                var adminUser = _userRepo.GetAll().FirstOrDefault(u => u.UserName == "admin");
                if (adminUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = "admin", EmailAddress = "admin@example.com",
                        Name = "Admin", Surname = "User",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                        IsActive = true,
                    };
                    adminUser.Id = _userRepo.InsertAndGetId(adminUser);
                    Console.WriteLine($"[Seed] Created admin user (id={adminUser.Id})");
                }
                if (!_userRoleRepo.GetAll().Any(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id))
                {
                    _userRoleRepo.Insert(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
                    Console.WriteLine("[Seed] Linked admin → Admin role");
                }

                Console.WriteLine("[Seed] Step 5: Custom role 'FinanceManager'");
                var customRole_0 = _roleRepo.GetAll().FirstOrDefault(r => r.Name == "FinanceManager");
                if (customRole_0 == null)
                {
                    customRole_0 = new AppRole { Name = "FinanceManager", DisplayName = "Finans Müdürü", Description = "Yönetici onayından geçmiş gider raporlarını nihai olarak onaylar veya revize ister; onay eşiklerini yönetir", IsSystem = false, IsActive = true };
                    customRole_0.Id = _roleRepo.InsertAndGetId(customRole_0);
                    Console.WriteLine($"[Seed] Created custom role 'FinanceManager' (id={customRole_0.Id})");
                }
                var customPerms_0 = new string[] { "ExpenseReport.Read", "ExpenseReport.Create", "ExpenseReport.Update", "ExpenseReport.Delete", "ExpenseReport.ChangeStatus", "ExpenseCategory.Read", "ExpenseCategory.Create", "ExpenseCategory.Update", "ExpenseCategory.Delete", "ExpenseItem.Read", "ExpenseApprovalThreshold.Read", "ExpenseApprovalThreshold.Create", "ExpenseApprovalThreshold.Update", "ExpenseApprovalThreshold.Delete" };
                var customGranted_0 = _rolePermRepo.GetAll().Where(p => p.RoleId == customRole_0.Id).Select(p => p.PermissionName).ToList();
                var allPermSet_0 = new HashSet<string>(_permRegistry.All.Select(p => p.Name));
                foreach (var pn in customPerms_0.Except(customGranted_0))
                {
                    if (!allPermSet_0.Contains(pn)) { Console.WriteLine($"[Seed] Skipping unknown permission '{pn}' for role 'FinanceManager'"); continue; }
                    _rolePermRepo.Insert(new RolePermission { RoleId = customRole_0.Id, PermissionName = pn });
                }

                Console.WriteLine("[Seed] Step 6: Custom role 'Employee'");
                var customRole_1 = _roleRepo.GetAll().FirstOrDefault(r => r.Name == "Employee");
                if (customRole_1 == null)
                {
                    customRole_1 = new AppRole { Name = "Employee", DisplayName = "Çalışan", Description = "Kendi gider raporlarını oluşturur, düzenler ve onaya gönderir", IsSystem = false, IsActive = true };
                    customRole_1.Id = _roleRepo.InsertAndGetId(customRole_1);
                    Console.WriteLine($"[Seed] Created custom role 'Employee' (id={customRole_1.Id})");
                }
                var customPerms_1 = new string[] { "ExpenseReport.Create", "ExpenseReport.Read", "ExpenseReport.Update", "ExpenseItem.Create", "ExpenseItem.Read", "ExpenseItem.Update", "ExpenseItem.Delete", "ExpenseCategory.Read" };
                var customGranted_1 = _rolePermRepo.GetAll().Where(p => p.RoleId == customRole_1.Id).Select(p => p.PermissionName).ToList();
                var allPermSet_1 = new HashSet<string>(_permRegistry.All.Select(p => p.Name));
                foreach (var pn in customPerms_1.Except(customGranted_1))
                {
                    if (!allPermSet_1.Contains(pn)) { Console.WriteLine($"[Seed] Skipping unknown permission '{pn}' for role 'Employee'"); continue; }
                    _rolePermRepo.Insert(new RolePermission { RoleId = customRole_1.Id, PermissionName = pn });
                }
                Console.WriteLine("[Seed] DONE — admin kullanicisi hazir");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Seed] FAILED: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[Seed] InnerException: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                Console.WriteLine("[Seed] StackTrace:");
                Console.WriteLine(ex.StackTrace);
                throw; // bubble up to MigrationHostedService catch
            }
        }
    }
}
