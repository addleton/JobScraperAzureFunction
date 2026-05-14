using JobScraper.App.Common;
using JobScraper.App.Features.Repository;
using Microsoft.Azure.Functions.Worker;

namespace JobScraper.App.Features.SendEmail;

public class SendEmailTimer
{
    private readonly EmailBuilder _builder;
    private readonly JobRepository _repo;
    private readonly EmailSender _sender;

    public SendEmailTimer(JobRepository repo, EmailBuilder builder, EmailSender sender)
    {
        _repo = repo;
        _builder = builder;
        _sender = sender;
    }

    [Function(nameof(SendEmailTimer))]
    public async Task RunTimer([TimerTrigger("0 0 6/24 * * *", RunOnStartup = true)] TimerInfo timerInfo,
        FunctionContext context)
    {
        List<JobPosting> jobPostings = await _repo.GetAllJobsWithoutEmailSentAsync();

        if (jobPostings.Count < 1) return;

        string email = _builder.BuildJobEmailHtml(jobPostings);

        bool success = await _sender.SendEmailAsync(email);

        if (success)
        {
            List<int> jobIds = jobPostings.Select(x => x.Id).ToList();

            await _repo.SetEmailSentTrueAsync(jobIds);
        }
    }
}