namespace JobScraper.App.Features.ScrapeAdzunaJobs;

public record AdzunaJobCompanyDto
{
    public string Display_Name { get; init; }
    public string Canonical_Name { get; init; }
    public int Count { get; init; }
    public decimal Average_Salary { get; init; }
}