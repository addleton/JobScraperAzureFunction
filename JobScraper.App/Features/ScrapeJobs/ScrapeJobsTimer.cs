using Microsoft.Azure.Functions.Worker;

namespace JobScraper.App.Features.ScrapeJobs;

public class ScrapeJobsTimer
{
    private readonly JobBoardsClient _jobBoardsClient;

    public ScrapeJobsTimer(JobBoardsClient jobBoardsClient)
    {
        _jobBoardsClient = jobBoardsClient;
    }

    [Function(nameof(ScrapeJobsTimer))]
    public async Task RunTimer([TimerTrigger("0 0 */4 * * *", RunOnStartup = true)] TimerInfo timerInfo,
        FunctionContext context)
    {
        var result = await _jobBoardsClient.GetReedJobPostings();
    }
}