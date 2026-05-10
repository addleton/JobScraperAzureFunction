using Microsoft.Azure.Functions.Worker;

namespace JobScraper.App.Features.ScrapeJobs;

public class ScrapeJobsTimer
{
    public static void RunTimer([TimerTrigger("0 0 */4 * * *")] TimerInfo timerInfo, FunctionContext context)
    {
        throw new NotImplementedException();
    }
}