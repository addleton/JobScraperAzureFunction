namespace JobScraper.App.Features.ScrapeJobs;

public record ReedJobDto
{
    public required int JobId { get; init; }
    public required string EmployerName { get; init; }
    public required string JobTitle { get; init; }
    public string? LocationName { get; init; }
    public decimal? MinimumSalary { get; init; } 
    public decimal? MaximumSalary { get; init; }
    public required string JobDescription { get; init; }
    public required string JobUrl { get; init; }
}