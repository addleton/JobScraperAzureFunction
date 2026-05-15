using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using JobScraper.App.Common;
using JobScraper.App.Features.Extensions;
using JobScraper.App.Features.Services;
using Microsoft.Extensions.Logging;

namespace JobScraper.App.Features.ScrapeReedJobs;

public class ReedApiClient
{
    private readonly HttpClient _client;
    private readonly JobFilterService _jobFilter;
    private readonly ILogger<ReedApiClient> _logger;

    private readonly string _reedApiKey = Environment.GetEnvironmentVariable("ReedApiKey") ??
                                          throw new NullReferenceException("Reed API Key was not found");

    private readonly string _reedJobsLocalSearch;
    private readonly string _reedJobsRemoteSearch;

    private readonly string _searchTerm = Uri.EscapeDataString("C# Developer");

    public ReedApiClient(IHttpClientFactory httpClientFactory, JobFilterService jobFilter,
        ILogger<ReedApiClient> logger)
    {
        _client = httpClientFactory.CreateClient();
        _jobFilter = jobFilter;
        _logger = logger;

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

    private async Task<List<JobPosting>> GetLocalReedJobPostings()
    {
        byte[] bytes = Encoding.ASCII.GetBytes($"{_reedApiKey}:");
        string base64String = Convert.ToBase64String(bytes);
        HttpRequestMessage request = new(HttpMethod.Get, _reedJobsLocalSearch);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
        HttpResponseMessage response = await _client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Reed API Error: {StatusCode}. Details: {ErrorMessage}", response.StatusCode,
                errorContent);
            throw new Exception($"Reed API Error: {response.StatusCode}. Details: {errorContent}");
        }

        ReedSearchResultDto? result = await response.Content.ReadFromJsonAsync<ReedSearchResultDto>();

        if (result is null) return [];

        List<JobPosting> jobs = result.Results.Select(job => job.ToModel()).ToList();

        return jobs;
    }

    private async Task<List<JobPosting>> GetRemoteReedJobPostings()
    {
        byte[] bytes = Encoding.ASCII.GetBytes($"{_reedApiKey}:");
        string base64String = Convert.ToBase64String(bytes);
        HttpRequestMessage request = new(HttpMethod.Get, _reedJobsRemoteSearch);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
        HttpResponseMessage response = await _client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Reed API Error: {StatusCode}. Details: {ErrorMessage}", response.StatusCode,
                errorContent);
            throw new Exception($"Reed API Error: {response.StatusCode}. Details: {errorContent}");
        }

        ReedSearchResultDto? result = await response.Content.ReadFromJsonAsync<ReedSearchResultDto>();

        if (result is null) return [];

        List<JobPosting> jobs = result.Results.Select(job => job.ToModel()).ToList();

        return jobs;
    }
}