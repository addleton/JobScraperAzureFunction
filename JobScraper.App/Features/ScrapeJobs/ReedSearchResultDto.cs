namespace JobScraper.App.Features.ScrapeJobs;

public record ReedSearchResultDto
{
    public required List<ReedJobDto> Results { get; init; } = [];
    public int TotalResults { get; init; }
}