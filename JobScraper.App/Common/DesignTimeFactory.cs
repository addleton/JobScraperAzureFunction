using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobScraper.App.Common;

public class DesignTimeFactory : IDesignTimeDbContextFactory<JobDbContext>
{
    public JobDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<JobDbContext> builder = new();
        string? connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        builder.UseSqlServer(connectionString, sqlOptions => { sqlOptions.EnableRetryOnFailure(); });
        return new JobDbContext(builder.Options);
    }
}