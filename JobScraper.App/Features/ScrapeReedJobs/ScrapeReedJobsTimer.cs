using JobScraper.App.Common;
using JobScraper.App.Features.Repository;
using Microsoft.Azure.Functions.Worker;

namespace JobScraper.App.Features.ScrapeReedJobs;

public class ScrapeReedJobsTimer
{
    private readonly ReedApiClient _reedApiClient;
    private readonly JobRepository _repository;

    public ScrapeReedJobsTimer(ReedApiClient reedApiClient, JobRepository repository)
    {
        _reedApiClient = reedApiClient;
        _repository = repository;
    }

    [Function(nameof(ScrapeReedJobsTimer))]
    public async Task RunTimer([TimerTrigger("0 0 */4 * * *", RunOnStartup = true)] TimerInfo timerInfo,
        FunctionContext context)
    {
        List<JobPosting> jobs = await _reedApiClient.GetAllReedJobPostings();

        if (jobs.Count < 1)
        {
            return;
        }

        await _repository.SaveJobsAsync(jobs);
    }
}