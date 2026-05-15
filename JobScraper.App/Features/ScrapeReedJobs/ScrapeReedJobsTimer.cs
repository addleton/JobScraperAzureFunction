using JobScraper.App.Common;
using JobScraper.App.Features.Repository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JobScraper.App.Features.ScrapeReedJobs;

public class ScrapeReedJobsTimer
{
    private readonly ReedApiClient _reedApiClient;
    private readonly JobRepository _repository;
    private readonly ILogger<ScrapeReedJobsTimer> _logger;

    public ScrapeReedJobsTimer(ReedApiClient reedApiClient, JobRepository repository,
        ILogger<ScrapeReedJobsTimer> logger)
    {
        _reedApiClient = reedApiClient;
        _repository = repository;
        _logger = logger;
    }

    [Function(nameof(ScrapeReedJobsTimer))]
    public async Task RunTimer([TimerTrigger("0 0 00/4 * * *")] TimerInfo timerInfo,
        FunctionContext context)
    {
        _logger.LogInformation("Starting Reed Job Scrape at {Time}:", DateTimeOffset.Now);
        List<JobPosting> jobs = await _reedApiClient.GetAllReedJobPostings();

        if (jobs.Count < 1)
        {
            _logger.LogInformation("No Reed jobs found.");
            return;
        }

        _logger.LogInformation("Found {Count} Reed jobs", jobs.Count);
        await _repository.SaveJobsAsync(jobs);
    }
}