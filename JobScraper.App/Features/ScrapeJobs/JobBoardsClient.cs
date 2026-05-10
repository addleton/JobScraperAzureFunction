using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using JobScraper.App.Common;

namespace JobScraper.App.Features.ScrapeJobs;

public class JobBoardsClient
{
    private readonly HttpClient _client;

    private readonly string _reedApiKey = Environment.GetEnvironmentVariable("ReedApiKey") ??
                                          throw new NullReferenceException("Reed API Key was not found");

    private readonly string _searchTerm = Uri.EscapeDataString("C# Developer");
    private readonly string _reedJobsLocalSearch;
    private readonly string _reedJobsRemoteSearch;

    public JobBoardsClient(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient();

        _reedJobsLocalSearch =
            $"https://www.reed.co.uk/api/1.0/search?keywords={_searchTerm}&location=S804JJ&distancefromlocation=30&minimumSalary=35000";
        _reedJobsRemoteSearch = $"https://www.reed.co.uk/api/1.0/search?keywords={_searchTerm}&minimumSalary=35000";
    }

    public async Task<List<JobPosting>> GetReedJobPostings()
    {
        Console.WriteLine(_reedApiKey);
        byte[] bytes = Encoding.ASCII.GetBytes($"{_reedApiKey}:");
        string base64String = Convert.ToBase64String(bytes);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);
        ReedSearchResultDto? result = await _client.GetFromJsonAsync<ReedSearchResultDto>(_reedJobsLocalSearch);

        if (result is null)
        {
            return [];
        }

        List<JobPosting> jobs = result.Results.Select(job => new JobPosting
        {
            Id = job.JobId,
            Title = job.JobTitle,
            Description = job.JobDescription,
            Company = job.EmployerName,
            Url = job.JobUrl,
            Salary = job.MinimumSalary ?? 0,
            EmailSent = false
        }).ToList();

        return jobs;
    }

    private List<JobPosting> GetRemoteOnlyJobPostings()
    {
        return [];
    }
}