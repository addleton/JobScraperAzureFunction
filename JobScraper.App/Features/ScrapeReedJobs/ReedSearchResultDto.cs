namespace JobScraper.App.Features.ScrapeReedJobs;

public record ReedSearchResultDto
{
    public required List<ReedJobDto> Results { get; init; } = [];
    public int TotalResults { get; init; }
}