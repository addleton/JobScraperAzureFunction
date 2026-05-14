using JobScraper.App.Common;
using JobScraper.App.Features.Repository;
using JobScraper.App.Features.ScrapeAdzunaJobs;
using JobScraper.App.Features.ScrapeReedJobs;
using JobScraper.App.Features.SendEmail;
using JobScraper.App.Features.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddHttpClient()
    .AddDbContext<JobDbContext>(options =>
    {
        string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString")
                                  ??
                                  "Server=(localdb)\\mssqllocaldb;Database=JobScraperLocal;Trusted_Connection=True;MultipleActiveResultSets=true";

        options.UseSqlServer(connectionString, sqlOptions => { sqlOptions.EnableRetryOnFailure(); });
    })
    .AddSingleton<ReedApiClient>()
    .AddSingleton<AdzunaApiClient>()
    .AddSingleton<JobFilterService>()
    .AddSingleton<EmailBuilder>()
    .AddSingleton<EmailSender>()
    .AddScoped<JobRepository>();

builder.Build().Run();