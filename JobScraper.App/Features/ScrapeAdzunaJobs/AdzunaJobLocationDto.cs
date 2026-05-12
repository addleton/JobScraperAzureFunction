namespace JobScraper.App.Features.ScrapeAdzunaJobs;

public record AdzunaJobLocationDto
{
    public string Display_Name { get; init; }
    public List<string> Area { get; init; }
}