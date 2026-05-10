using Microsoft.EntityFrameworkCore;

namespace JobScraper.App.Common;

public class JobDbContext : DbContext
{
    public JobDbContext(DbContextOptions<JobDbContext> options) : base(options)
    {
    }

    public DbSet<JobPosting> JobPostings { get; set; }
}