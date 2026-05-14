using JobScraper.App.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobScraper.App.Features.Repository;

public class JobRepository
{
    private readonly JobDbContext _context;

    public JobRepository(JobDbContext context)
    {
        _context = context;
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
            Console.WriteLine(e);
            return null;
        }
    }
    }
}