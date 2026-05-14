using JobScraper.App.Common;

namespace JobScraper.App.Features.Services;

public class JobFilterService
{
    public List<JobPosting> FilterRemoteJobs(List<JobPosting> jobs)
    {
        List<JobPosting> filteredJobs = [];

        foreach (JobPosting job in jobs)
            if (job.Description.Contains("remote", StringComparison.OrdinalIgnoreCase))
                filteredJobs.Add(job);

        return filteredJobs;
    }
}