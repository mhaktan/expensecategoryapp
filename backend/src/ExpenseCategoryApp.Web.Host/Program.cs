using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ExpenseCategoryApp.EntityFrameworkCore;
using ExpenseCategoryApp.EntityFrameworkCore.Seed;
using ExpenseCategoryApp.Entities;

namespace ExpenseCategoryApp.Web.Host
{
    /// <summary>
    /// Background service that runs migration + seed once at startup
    /// without blocking the HTTP pipeline.
    /// </summary>
    public class MigrationHostedService : IHostedService
    {
        private readonly IConfiguration _config;

        public MigrationHostedService(IConfiguration config)
        {
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var connStr = _config.GetConnectionString("Default") ?? "";
            if (string.IsNullOrEmpty(connStr)) return;

            // Run in background to avoid blocking host startup (prevents EF tooling timeout)
            _ = Task.Run(async () =>
            {
                // Small delay to ensure host is fully started before DB operations
                await Task.Delay(1000, cancellationToken);
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseNpgsql(connStr);

                    using (var db = new ExpenseCategoryAppDbContext(optionsBuilder.Options))
                    {
                        // EnsureCreated bos veritabaninda en guncel semayi kurar ve true doner.
                        // Dolu veritabaninda hicbir sey yapmaz — o durumda bekleyen migration'lar
                        // SchemaMigrations.Apply icinde sirayla calistirilir.
                        var freshlyCreated = db.Database.EnsureCreated();
                        Console.WriteLine("[Migration] Migration tanimi yok — sema EnsureCreated ile yonetiliyor.");
                        Console.WriteLine("[Migration] Database is up to date.");

                        // Seed sample data — wrapped in its own try so a failure here doesn't block RBAC seed below.
                        try
                        {
                    if (!db.ExpenseCategories.Any())
                    {
                        db.ExpenseCategories.AddRange(
                    new ExpenseCategory { Id = 1, Name = "Gider Kategorisi 1", Description = "Lorem ipsum dolor sit amet", IsActive = true },
                    new ExpenseCategory { Id = 2, Name = "Gider Kategorisi 2", Description = "Consectetur adipiscing elit", IsActive = false }
                        );
                    }
                    if (!db.ExpenseApprovalThresholds.Any())
                    {
                        db.ExpenseApprovalThresholds.AddRange(
                    new ExpenseApprovalThreshold { Id = 3, Name = "Gider Onay Eşiği 1", Currency = (ExpenseApprovalThresholdCurrency)0, ThresholdAmount = 99.99m, Description = "Lorem ipsum dolor sit amet", IsActive = true },
                    new ExpenseApprovalThreshold { Id = 4, Name = "Gider Onay Eşiği 2", Currency = (ExpenseApprovalThresholdCurrency)1, ThresholdAmount = 149.50m, Description = "Consectetur adipiscing elit", IsActive = false }
                        );
                    }
                    if (!db.ExpenseReports.Any())
                    {
                        db.ExpenseReports.AddRange(
                    new ExpenseReport { Id = 5, Title = "Introduction to Physics", Description = "Lorem ipsum dolor sit amet", ReportDate = new DateTime(2024, 3, 15), TotalAmount = 99.99m, Currency = (ExpenseReportCurrency)0, Status = (ExpenseReportStatus)0, RevisionNote = "Lorem ipsum dolor sit amet" },
                    new ExpenseReport { Id = 6, Title = "Advanced Mathematics", Description = "Consectetur adipiscing elit", ReportDate = new DateTime(2024, 6, 20), TotalAmount = 149.50m, Currency = (ExpenseReportCurrency)1, Status = (ExpenseReportStatus)0, RevisionNote = "Consectetur adipiscing elit" }
                        );
                    }
                    if (!db.ExpenseItems.Any())
                    {
                        db.ExpenseItems.AddRange(
                    new ExpenseItem { Id = 7, Description = "Lorem ipsum dolor sit amet", ExpenseDate = new DateTime(2024, 3, 15), Amount = 99.99m, Currency = (ExpenseItemCurrency)0, ExchangeRate = 99.99m, AmountInTRY = 99.99m, ReceiptNo = "ABC-001", ExpenseReportId = 5, ExpenseCategoryId = 1 },
                    new ExpenseItem { Id = 8, Description = "Consectetur adipiscing elit", ExpenseDate = new DateTime(2024, 6, 20), Amount = 149.50m, Currency = (ExpenseItemCurrency)1, ExchangeRate = 149.50m, AmountInTRY = 149.50m, ReceiptNo = "XYZ-002", ExpenseReportId = 6, ExpenseCategoryId = 2 }
                        );
                    }
                            db.SaveChanges();
                            Console.WriteLine("[Seed] Sample data created.");
                        }
                        catch (Exception sampleEx)
                        {
                            Console.WriteLine($"[Seed] Sample data skipped: {sampleEx.GetType().Name}: {sampleEx.Message}");
                            // Carry on — RBAC seed must still run so admin/123qwe is usable.
                        }
                        // Sync identity sequences to MAX(Id). Seeded rows carry explicit Ids which do NOT
                        // advance Postgres identity sequences → nextval collides with a seed row and the
                        // first few inserts fail with a duplicate-key 500. Runs every startup; idempotent.
                        try
                        {
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"ExpenseCategories\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"ExpenseCategories\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"ExpenseApprovalThresholds\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"ExpenseApprovalThresholds\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"ExpenseReports\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"ExpenseReports\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"ExpenseItems\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"ExpenseItems\") + 1, false);");
                            Console.WriteLine("[Seed] Identity sequences synced.");
                        }
                        catch (Exception seqEx)
                        {
                            Console.WriteLine($"[Seed] Sequence sync skipped: {seqEx.GetType().Name}: {seqEx.Message}");
                        }
                    }
                    // RBAC seed (Admin/User roles + permissions + admin user) runs through ABP DI
                    // so PermissionRegistry can be injected. SeedHelper is idempotent.
                    SeedHelper.SeedHostDb(Abp.Dependency.IocManager.Instance);
                    Console.WriteLine("[Seed] RBAC seed complete (Admin role + admin user).");
                }
                catch (Exception ex)
                {
                    // Full diagnostic — surface the real cause so silent seed failures are debuggable.
                    Console.WriteLine($"[Migration] FAILED: {ex.GetType().Name}: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Migration] InnerException: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                    Console.WriteLine("[Migration] StackTrace:");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("[Migration] App continues without migration — admin user will not exist.");
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class Program
    {
        // Runtime entry: WebHost is required because ABP Startup returns IServiceProvider.
        public static void Main(string[] args)
        {
            // Npgsql 7+ requires UTC DateTimes — enable legacy behavior for ABP compatibility
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        // Design-time entry for EF Core tools (dotnet ef migrations).
        // Without this, EF tools wait 5 minutes for IHost build (resolver default timeout)
        // and then SIGTERM any running dotnet process — killing live dev servers.
        // We expose a minimal IHost that EF tools resolve in milliseconds; the actual
        // DbContext is built by IDesignTimeDbContextFactory in the EntityFrameworkCore project.
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
    }
}
