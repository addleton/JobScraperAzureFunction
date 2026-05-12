using JobScraper.App.Common;
using JobScraper.App.Features.Repository;
using Microsoft.Azure.Functions.Worker;

namespace JobScraper.App.Features.ScrapeAdzunaJobs;

public class ScrapeAdzunaJobsTimer
{
    private readonly AdzunaApiClient _adzunaApiClient;
    private readonly JobRepository _jobRepository;

    public ScrapeAdzunaJobsTimer(AdzunaApiClient adzunaApiClient, JobRepository repository)
    {
        _adzunaApiClient = adzunaApiClient;
        _jobRepository = repository;
    }


    [Function(nameof(ScrapeAdzunaJobsTimer))]
    public async Task RunTimer([TimerTrigger("0 0 */4 * * *", RunOnStartup = true)] TimerInfo timerInfo,
        FunctionContext context)
    {
        List<JobPosting> jobs = await _adzunaApiClient.GetAdzunaJobPostings();

        if (jobs.Count < 1)
        {
            return;
        }

        await _jobRepository.SaveJobsAsync(jobs);
    }
}