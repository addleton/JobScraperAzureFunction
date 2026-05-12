using JobScraper.App.Common;
using JobScraper.App.Features.ScrapeAdzunaJobs;
using JobScraper.App.Features.ScrapeReedJobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobScraper.App.Features.Extensions;

public static class DtoExtensions
{
    public static JobPosting ToModel(this AdzunaJobDto dto)
    {
        return new JobPosting()
        {
            JobId = dto.Id,
            Source = "Adzuna",
            Title = dto.Title,
            Description = dto.Description,
            Company = dto.Company.Display_Name,
            Url = dto.Redirect_Url,
            Location = dto.Location.Display_Name,
            Salary = dto.Salary_Min,
            EmailSent = false
        };
    }

    public static JobPosting ToModel(this ReedJobDto dto)
    {
        return new JobPosting()
        {
            JobId = dto.JobId.ToString(),
            Source = "Reed",
            Title = dto.JobTitle,
            Description = dto.JobDescription,
            Company = dto.EmployerName,
            Url = dto.JobUrl,
            Location = dto.LocationName,
            Salary = dto.MinimumSalary ?? 0,
            EmailSent = false
        };
    }
}
