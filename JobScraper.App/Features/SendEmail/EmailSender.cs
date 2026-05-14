using Azure;
using Azure.Communication.Email;

namespace JobScraper.App.Features.SendEmail;

public class EmailSender
{
    private readonly string _connectionString;
    private readonly string _emailRecipient;
    private readonly string _emailSender;

    public EmailSender()
    {
        _connectionString = Environment.GetEnvironmentVariable("AzureCommConnectionString") ??
                            throw new NullReferenceException("Azure Email Connection string not found");
        _emailSender = Environment.GetEnvironmentVariable("AzureEmail") ??
                       throw new NullReferenceException("Azure Email Address not found");
        _emailRecipient = Environment.GetEnvironmentVariable("PersonalEmail") ??
                          throw new NullReferenceException("Personal Email Address not found");
    }

    public async Task<bool> SendEmailAsync(string htmlContent)
    {
        EmailClient client = new(_connectionString);

        EmailContent content = new("New Jobs Found")
        {
            Html = htmlContent
        };

        EmailMessage message = new(_emailSender, _emailRecipient, content);

        try
        {
            await client.SendAsync(WaitUntil.Completed, message);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occured while sending email", ex);
        }
    }
}