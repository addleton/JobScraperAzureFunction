using System.ComponentModel.DataAnnotations;

namespace JobScraper.App.Common;

public class JobPosting
{
    public int Id { get; init; }
    [Required] [MaxLength(100)] public required string Title { get; init; }
    [Required] [MaxLength(10000)] public required string Description { get; init; }
    [Required] [MaxLength(100)] public required string Company { get; init; }
    [Required] [MaxLength(1000)] public required string Url { get; init; }
    public decimal? Salary { get; init; }
    public bool EmailSent { get; init; }
}