namespace JobScraper.App.Features.ScrapeAdzunaJobs;

public record AdzunaJobDto
{
    public string Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Created { get; init; }
    public string Redirect_Url { get; init; }
    public string Adref { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public AdzunaJobLocationDto Location { get; init; }
    public AdzunaJobCategoryDto Category { get; init; }
    public AdzunaJobCompanyDto Company { get; init; }
    public decimal Salary_Min { get; init; }
    public decimal Salary_Max { get; init; }
    public string Salary_Is_Predicted { get; init; }
    public string Contract_Time { get; init; }
    public string Contract_Type { get; init; }
}