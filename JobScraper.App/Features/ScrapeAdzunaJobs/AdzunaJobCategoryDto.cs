namespace JobScraper.App.Features.ScrapeAdzunaJobs;

public record AdzunaJobCategoryDto
{
    public string Tag { get; init; }
    public string Label { get; init; }
}