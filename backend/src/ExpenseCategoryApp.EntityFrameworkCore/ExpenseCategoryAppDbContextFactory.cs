using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExpenseCategoryApp.EntityFrameworkCore
{
    public class ExpenseCategoryAppDbContextFactory : IDesignTimeDbContextFactory<ExpenseCategoryAppDbContext>
    {
        public ExpenseCategoryAppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ExpenseCategoryApp.Web.Host"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connStr = configuration.GetConnectionString("Default");
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(connStr);
            return new ExpenseCategoryAppDbContext(builder.Options);
        }
    }
}
