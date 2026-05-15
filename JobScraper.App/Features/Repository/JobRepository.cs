using JobScraper.App.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobScraper.App.Features.Repository;

public class JobRepository
{
    private readonly JobDbContext _context;
    private readonly ILogger<JobRepository> _logger;

    public JobRepository(JobDbContext context, ILogger<JobRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SaveJobsAsync(List<JobPosting> jobs)
    {
        List<string> newIdList = jobs.Select(x => x.JobId).ToList();
        List<string> newSources = jobs.Select(x => x.Source).Distinct().ToList();

        var existingCompositeList = await _context.JobPostings
            .Where(j => newIdList.Contains(j.JobId) && newSources.Contains(j.Source))
            .Select(j => new { j.JobId, j.Source })
            .ToListAsync();

        List<JobPosting> newJobs = jobs.Where(newJob =>
            !existingCompositeList.Any(existing =>
                existing.Source == newJob.Source && existing.JobId == newJob.JobId)
        ).ToList();

        if (newJobs.Count > 0)
        {
            _logger.LogInformation("Adding {Count} jobs for job postings", newJobs.Count);
            await _context.JobPostings.AddRangeAsync(newJobs);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<JobPosting>> GetAllJobsWithoutEmailSentAsync()
    {
        try
        {
            return await _context.JobPostings.Where(job => !job.EmailSent).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting jobs for job postings");
            return [];
        }
    }

    public async Task SetEmailSentTrueAsync(List<int> jobIds)
    {
        List<JobPosting> jobs = await _context.JobPostings.Where((j => jobIds.Contains(j.Id))).ToListAsync();

        foreach (JobPosting job in jobs)
        {
            _logger.LogInformation("Setting email sent to true for job {Id}", job.Id);
            job.EmailSent = true;
        }

        await _context.SaveChangesAsync();
    }
}