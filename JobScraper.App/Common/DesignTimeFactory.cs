using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobScraper.App.Common;

public class DesignTimeFactory : IDesignTimeDbContextFactory<JobDbContext>
{
    public JobDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<JobDbContext>();
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=JobScraperLocal;Trusted_Connection=True;MultipleActiveResultSets=true";
        builder.UseSqlServer(connectionString);
        return new JobDbContext(builder.Options);
    }
}