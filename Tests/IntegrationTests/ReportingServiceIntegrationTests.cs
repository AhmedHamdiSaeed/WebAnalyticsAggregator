using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Implementations;
using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.interfaces;
using Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Tests.IntegrationTests
{
    public class ReportingServiceIntegrationTests : IDisposable
    {
        private readonly AnalyticsDbContext _dbContext;
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly ReportService _reportService;

        public ReportingServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AnalyticsDbContext>()
               .UseSqlite("DataSource=:memory:")  // In-memory SQLite
                .Options;

            _dbContext = new AnalyticsDbContext(options);
            _dbContext.Database.OpenConnection();     // Needed for in-memory SQLite
            _dbContext.Database.EnsureCreated();      // Create schema

            // Seed some data
            _dbContext.DailyStats.AddRange(new List<DailyStats>
            {
                new DailyStats { Date = DateTime.Today, TotalUsers = 100, TotalSessions = 200, TotalViews = 300, AvgPerformance = 0.9m },
                new DailyStats { Date = DateTime.Today, TotalUsers = 150, TotalSessions = 250, TotalViews = 350, AvgPerformance = 0.8m }
            });
            _dbContext.SaveChanges();

            // Setup repository and service
            _analyticsRepository = new AnalyticsRepository(_dbContext);
            _reportService = new ReportService(_analyticsRepository);
        }

        [Fact]
        public async Task GetOverviewReportAsync_ShouldReturnAggregatedValues()
        {
            // Act
            var result = await _reportService.GetOverviewReportAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(250, result.Data.TotalUsers);
            Assert.Equal(450, result.Data.TotalSessions);
            Assert.Equal(650, result.Data.TotalViews);
            Assert.Equal(0.85m, result.Data.AvgPerformance, 2);
        }

        [Fact]
        public async Task GetPerPageReportAsync_ShouldReturnAllPages()
        {
            // Seed page data
            _dbContext.RawData.AddRange(new List<CombinedRecord>
            {
                new CombinedRecord { Date = DateTime.Today, Page = "/home", Users = 100, Sessions = 200, Views = 300, PerformanceScore = 0.9m },
                new CombinedRecord { Date = DateTime.Today, Page = "/about", Users = 50, Sessions = 100, Views = 150, PerformanceScore = 0.8m }
            });
            _dbContext.SaveChanges();

            // Act
            var result = await _reportService.GetPerPageReportAsync();

            // Assert
            Assert.Equal(2, result.Count());
            var homePage = result.First(r => r.Page == "/home");
            Assert.Equal(100, homePage.TotalUsers);
            Assert.Equal(0.9m, homePage.AvgPerformance);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
