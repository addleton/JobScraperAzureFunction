using JobScraper.App.Common;
using JobScraper.App.Features.Repository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JobScraper.App.Features.SendEmail;

public class SendEmailTimer
{
    private readonly EmailBuilder _builder;
    private readonly JobRepository _repo;
    private readonly EmailSender _sender;
    private readonly ILogger<SendEmailTimer> _logger;

    public SendEmailTimer(JobRepository repo, EmailBuilder builder, EmailSender sender, ILogger<SendEmailTimer> logger)
    {
        _repo = repo;
        _builder = builder;
        _sender = sender;
        _logger = logger;
    }

    [Function(nameof(SendEmailTimer))]
    public async Task RunTimer([TimerTrigger("0 0 6/24 * * *")] TimerInfo timerInfo,
        FunctionContext context)
    {
        _logger.LogInformation("Starting Email Sender at {Time}:", DateTimeOffset.Now);
        List<JobPosting> jobPostings = await _repo.GetAllJobsWithoutEmailSentAsync();

        if (jobPostings.Count < 1)
        {
            _logger.LogInformation("No new jobs found.");
            return;
        }

        string email = _builder.BuildJobEmailHtml(jobPostings);

        _logger.LogInformation("Sending jobs email");
        bool success = await _sender.SendEmailAsync(email);

        if (!success)
        {
            _logger.LogError("Failed to send jobs email.");
            return;
        }

        _logger.LogInformation("Email sent successfully.");
        List<int> jobIds = jobPostings.Select(x => x.Id).ToList();
        await _repo.SetEmailSentTrueAsync(jobIds);
    }
}