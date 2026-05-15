using System.Net.Http.Json;
using JobScraper.App.Common;
using JobScraper.App.Features.Extensions;
using JobScraper.App.Features.Services;
using Microsoft.Extensions.Logging;

namespace JobScraper.App.Features.ScrapeAdzunaJobs;

public class AdzunaApiClient
{
    private readonly HttpClient _client;
    private readonly JobFilterService _filter;
    private readonly string _localAdzunaSearch;
    private readonly string _remoteAdzunaSearch;
    private readonly ILogger<AdzunaApiClient> _logger;


    public AdzunaApiClient(IHttpClientFactory httpClientFactory, JobFilterService filter,
        ILogger<AdzunaApiClient> logger)
    {
        _client = httpClientFactory.CreateClient();
        _filter = filter;
        _logger = logger;

        string appId = Environment.GetEnvironmentVariable("AdzunaAppId") ??
                       throw new NullReferenceException("AdzunaAppId not found");
        string apiKey = Environment.GetEnvironmentVariable("AdzunaApiKey") ??
                        throw new NullReferenceException("AdzunaApiKey not found");
        _localAdzunaSearch =
            $"https://api.adzuna.com/v1/api/jobs/gb/search/1?app_id={appId}&app_key={apiKey}&results_per_page=100&what=C%23%20Developer&where=Worksop&distance=40&max_days_old=1&salary_min=35000&salary_include_unknown=1&sort_by=date";
        _remoteAdzunaSearch =
            $"https://api.adzuna.com/v1/api/jobs/gb/search/1?app_id={appId}&app_key={apiKey}&results_per_page=100&what=C%23%20Developer&max_days_old=1&salary_min=35000&salary_include_unknown=1&sort_by=date";
    }

    public async Task<List<JobPosting>> GetAdzunaJobPostings()
    {
        List<JobPosting> localJobs = await GetLocalAdzunaJobPostings();
        List<JobPosting> remoteJobs = await GetRemoteAdzunaJobPostings();
        List<JobPosting> filteredJobs = _filter.FilterRemoteJobs(remoteJobs);
        return localJobs.UnionBy(filteredJobs, job => job.JobId).ToList();
    }

    private async Task<List<JobPosting>> GetLocalAdzunaJobPostings()
    {
        HttpResponseMessage response = await _client.GetAsync(_localAdzunaSearch);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Adzuna API Error: {StatusCode}. Details: {ErrorMessage}", response.StatusCode,
                errorContent);
            throw new Exception($"Adzuna API Error: {response.StatusCode}. Details: {errorContent}");
        }

        if (response.Content.Headers.ContentType?.CharSet == "utf8")
            response.Content.Headers.ContentType.CharSet = "utf-8";

        AdzunaSearchResultDto? result = await response.Content.ReadFromJsonAsync<AdzunaSearchResultDto>();

        if (result is null) return [];

        List<JobPosting> jobs = result.Results.Select(job => job.ToModel()).ToList();

        return jobs;
    }

    private async Task<List<JobPosting>> GetRemoteAdzunaJobPostings()
    {
        HttpResponseMessage response = await _client.GetAsync(_remoteAdzunaSearch);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Adzuna API Error: {StatusCode}. Details: {ErrorMessage}", response.StatusCode,
                errorContent);
            throw new Exception($"Adzuna API Error: {response.StatusCode}. Details: {errorContent}");
        }

        if (response.Content.Headers.ContentType?.CharSet == "utf8")
            response.Content.Headers.ContentType.CharSet = "utf-8";

        AdzunaSearchResultDto? result = await response.Content.ReadFromJsonAsync<AdzunaSearchResultDto>();

        if (result is null) return [];

        List<JobPosting> jobs = result.Results.Select(job => job.ToModel()).ToList();

        return jobs;
    }
}