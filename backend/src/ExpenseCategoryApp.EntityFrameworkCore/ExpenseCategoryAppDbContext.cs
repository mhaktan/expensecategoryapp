using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore;
using ExpenseCategoryApp.Entities;

namespace ExpenseCategoryApp.EntityFrameworkCore
{
    public class ExpenseCategoryAppDbContext : AbpDbContext
    {
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<ExpenseApprovalThreshold> ExpenseApprovalThresholds { get; set; }
        public DbSet<ExpenseReport> ExpenseReports { get; set; }
        public DbSet<ExpenseItem> ExpenseItems { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ApprovalRecord> ApprovalRecords { get; set; }
        public DbSet<StatusChangeLog> StatusChangeLogs { get; set; }


        public ExpenseCategoryAppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ExpenseReport 1:N ExpenseItem
            modelBuilder.Entity<ExpenseItem>()
                .HasOne(x => x.ExpenseReport)
                .WithMany(x => x.ExpenseItems)
                .HasForeignKey(x => x.ExpenseReportId)
                .OnDelete(DeleteBehavior.Cascade);

            // ExpenseCategory 1:N ExpenseItem
            modelBuilder.Entity<ExpenseItem>()
                .HasOne(x => x.ExpenseCategory)
                .WithMany(x => x.ExpenseItems)
                .HasForeignKey(x => x.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // RBAC: AppUser N:N AppRole via UserRole junction
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // RolePermission: AppRole 1:N RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionName })
                .IsUnique();

            // AppRole.Name unique
            modelBuilder.Entity<AppRole>()
                .HasIndex(r => r.Name)
                .IsUnique();

        }
    }
}
