# 🚀 JobScraper (Azure Serverless)

An automated, serverless background worker built with **.NET (Azure Functions)** that continuously scrapes job boards, deduplicates listings, and delivers freshly found roles straight to your inbox.

## ✨ Features
* **Multi-Source Scraping:** Integrates with both the [Reed.co.uk API](https://www.reed.co.uk/developers) and the [Adzuna API](https://developer.adzuna.com/) to fetch the latest job postings.
* **Smart Deduplication:** Uses Entity Framework Core and an Azure SQL Database to ensure you are never emailed the same job twice.
* **Automated Scheduling:** Runs fully autonomously in the cloud using Azure Function Time Triggers (CRON scheduling).
* **HTML Email Delivery:** Compiles clean, responsive HTML templates and delivers them via Azure Communication Services.
* **Serverless Architecture:** Deployed on Azure's Consumption Plan for infinite scalability and zero idle costs.

## 🛠️ Technologies Used
* **C# / .NET** (Isolated Worker Model)
* **Azure Functions** (Serverless Compute)
* **Azure SQL Database** (Relational Data)
* **Entity Framework Core** (ORM & Migrations)
* **Azure Communication Services** (Email Delivery)
* **Dependency Injection** (Scoped Repositories, Singleton API Clients)

## 🏗️ Local Development Setup

### Prerequisites
* .NET SDK
* JetBrains Rider or Visual Studio
* Azure Storage Emulator (Azurite)
* SQL Server Express / LocalDB

### 1. Clone the repository
    git clone https://github.com/yourusername/JobScraper.git
    cd JobScraper

### 2. Configure Environment Variables
Create a `local.settings.json` file in the `JobScraper.App` root directory. **Note: This file is ignored by Git to protect your API keys.**

    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "SqlConnectionString": "Server=(localdb)\\mssqllocaldb;Database=JobScraperLocal;Trusted_Connection=True",
        "ReedApiKey": "YOUR_REED_API_KEY",
        "AdzunaAppId": "YOUR_ADZUNA_APP_ID",
        "AdzunaAppKey": "YOUR_ADZUNA_APP_KEY",
        "AzureCommConnectionString": "endpoint=https://your-service.communication.azure.com/;accesskey=...",
        "AzureEmail": "donotreply@xxxxxxxxxxxx.azurecomm.net",
        "PersonalEmail": "your.email@gmail.com"
      }
    }

### 3. Run Database Migrations
The project uses EF Core Code-First migrations. Apply the schema to your local database by running:

    dotnet ef database update

### 4. Run the App
Start the Azure Functions runtime locally. You can use your IDE's built-in Azure tools or the command line:

    func start

## ☁️ Cloud Deployment (Azure)
This application is designed to be deployed to an **Azure Function App (Windows)** on the **Consumption Plan**.

When deploying to Azure, ensure the following Application Settings (Environment Variables) are configured in the Azure Portal:
* All API keys and connection strings from your `local.settings.json`
* `WEBSITE_TIME_ZONE` set to `GMT Standard Time` (to ensure CRON schedules respect UK daylight saving time).

---
*Developed by Michael Addleton*
