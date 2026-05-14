using System.Text;
using JobScraper.App.Common;

namespace JobScraper.App.Features.SendEmail;

public class EmailBuilder
{
    public string BuildJobEmailHtml(List<JobPosting> jobs)
    {
        const string rowTemplate = """
                                   <tr>
                                      <td style='border-bottom: 1px solid #eeeeee; padding: 20px 0;'>
                                          <h2 style='margin: 0 0 5px 0; font-size: 18px; color: #0078D4;'>{TITLE}</h2>
                                          <p style='margin: 0 0 10px 0; font-size: 14px; color: #555555;'>
                                              <strong>{COMPANY}</strong> &bull; {LOCATION} &bull; £{SALARY}
                                          </p>
                                          <a href='{URL}' target='_blank' style='display: inline-block; padding: 10px 18px; background-color: #28a745; color: white; text-decoration: none; border-radius: 4px; font-size: 14px; font-weight: bold;'>View Job Posting</a>
                                      </td>
                                   </tr>
                                   """;

        StringBuilder jobRowsHtml = new();
        foreach (string html in jobs.Select(job => rowTemplate
                     .Replace("{TITLE}", job.Title)
                     .Replace("{COMPANY}", job.Company)
                     .Replace("{LOCATION}", job.Location)
                     .Replace("{SALARY}", job.Salary.ToString())
                     .Replace("{URL}", job.Url)))
            jobRowsHtml.AppendLine(html);

        const string baseTemplate = """
                                    <!DOCTYPE html>
                                    <html>
                                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px;'>
                                      <table width='100%' cellpadding='0' cellspacing='0' border='0' style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                                          <tr>
                                              <td style='background-color: #0078D4; padding: 20px; text-align: center; color: white;'>
                                                  <h1 style='margin: 0; font-size: 24px;'>🚀 New Jobs Found!</h1>
                                              </td>
                                          </tr>
                                          <tr>
                                              <td style='padding: 20px;'>
                                                  <p style='color: #333333; font-size: 16px; margin-top: 0;'>Here are the latest job postings matching your criteria:</p>
                                                  <table width='100%' cellpadding='0' cellspacing='0' border='0' style='border-collapse: collapse;'>
                                                      <tbody>
                                                          {{JOB_ROWS_TOKEN}}
                                                      </tbody>
                                                  </table>
                                              </td>
                                          </tr>
                                      </table>
                                    </body>
                                    </html>
                                    """;

        return baseTemplate.Replace("{{JOB_ROWS_TOKEN}}", jobRowsHtml.ToString());
    }
}