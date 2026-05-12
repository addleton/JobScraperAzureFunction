using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using JobScraper.App.Common;
using JobScraper.App.Features.Extensions;
using JobScraper.App.Features.Services;

namespace JobScraper.App.Features.ScrapeReedJobs;

public class ReedApiClient
{
    private readonly HttpClient _client;
    private readonly JobFilterService _jobFilter;

    private readonly string _reedApiKey = Environment.GetEnvironmentVariable("ReedApiKey") ??
                                          throw new NullReferenceException("Reed API Key was not found");

    private readonly string _searchTerm = Uri.EscapeDataString("C# Developer");
    private readonly string _reedJobsLocalSearch;
    private readonly string _reedJobsRemoteSearch;

    public ReedApiClient(IHttpClientFactory httpClientFactory, JobFilterService jobFilter)
    {
        _client = httpClientFactory.CreateClient();
        _jobFilter = jobFilter;

        _reedJobsLocalSearch =
            $"https://www.reed.co.uk/api/1.0/search?keywords={_searchTerm}&locationName=S804JJ&distancefromlocation=30&minimumSalary=35000";
        _reedJobsRemoteSearch = $"https://www.reed.co.uk/api/1.0/search?keywords={_searchTerm}&minimumSalary=35000";
    }

    public async Task<List<JobPosting>> GetAllReedJobPostings()
    {
        List<JobPosting> localJobs = await GetLocalReedJobPostings();
        List<JobPosting> remoteJobs = await GetRemoteReedJobPostings();
        List<JobPosting> filteredJobs = _jobFilter.FilterRemoteJobs(remoteJobs);
        return localJobs.UnionBy(filteredJobs, job => job.JobId).ToList();
    }

    public async Task<List<JobPosting>> GetLocalReedJobPostings()
    {
        byte[] bytes = Encoding.ASCII.GetBytes($"{_reedApiKey}:");
        string base64String = Convert.ToBase64String(bytes);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

        HttpResponseMessage response = await _client.GetAsync(_reedJobsLocalSearch);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Reed API Error: {response.StatusCode}. Details: {errorContent}");
        }

        ReedSearchResultDto? result = await response.Content.ReadFromJsonAsync<ReedSearchResultDto>();

        if (result is null)
        {
            return [];
        }

        List<JobPosting> jobs = result.Results.Select(job => job.ToModel()).ToList();

        return jobs;
    }

    private async Task<List<JobPosting>> GetRemoteReedJobPostings()
    {
        byte[] bytes = Encoding.ASCII.GetBytes($"{_reedApiKey}:");
        string base64String = Convert.ToBase64String(bytes);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

        HttpResponseMessage response = await _client.GetAsync(_reedJobsRemoteSearch);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Reed API Error: {response.StatusCode}. Details: {errorContent}");
        }

        ReedSearchResultDto? result = await response.Content.ReadFromJsonAsync<ReedSearchResultDto>();

        if (result is null)
        {
            return [];
        }

        List<JobPosting> jobs = result.Results.Select(job => job.ToModel()).ToList();

        return jobs;
    }
}