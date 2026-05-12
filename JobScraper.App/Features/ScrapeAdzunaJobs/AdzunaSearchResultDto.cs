namespace JobScraper.App.Features.ScrapeAdzunaJobs;

public record AdzunaSearchResultDto
{
    public int Count { get; set; }
    public decimal Mean { get; set; }
    public List<AdzunaJobDto> Results { get; set; }
}