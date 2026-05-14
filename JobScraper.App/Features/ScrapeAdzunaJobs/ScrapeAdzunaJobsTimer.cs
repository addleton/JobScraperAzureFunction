using JobScraper.App.Common;
using JobScraper.App.Features.Repository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JobScraper.App.Features.ScrapeAdzunaJobs;

public class ScrapeAdzunaJobsTimer
{
    private readonly AdzunaApiClient _adzunaApiClient;
    private readonly JobRepository _jobRepository;
    private readonly ILogger<ScrapeAdzunaJobsTimer> _logger;

    public ScrapeAdzunaJobsTimer(AdzunaApiClient adzunaApiClient, JobRepository repository,
        ILogger<ScrapeAdzunaJobsTimer> logger)
    {
        _adzunaApiClient = adzunaApiClient;
        _jobRepository = repository;
        _logger = logger;
    }


    [Function(nameof(ScrapeAdzunaJobsTimer))]
    public async Task RunTimer([TimerTrigger("0 0 00/4 * * *")] TimerInfo timerInfo,
        FunctionContext context)
    {
        _logger.LogInformation("Starting Adzuna Job Scrape at {Time}:", DateTimeOffset.Now);
        List<JobPosting> jobs = await _adzunaApiClient.GetAdzunaJobPostings();

        if (jobs.Count < 1)
        {
            _logger.LogInformation("No Adzuna jobs found.");
            return;
        }

        _logger.LogInformation("Found {Count} Adzuna jobs", jobs.Count);
        await _jobRepository.SaveJobsAsync(jobs);
    }
}