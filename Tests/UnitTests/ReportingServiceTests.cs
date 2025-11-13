using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Implementations;
using Application.Interfaces;
using DTOs.Reports;
using Infrastructure.Entities;
using Infrastructure.interfaces;
using Moq;

namespace Tests.UnitTests
{
    public class ReportingServiceTests
    {

        private readonly Mock<IAnalyticsRepository> _analyticsRepoMock;
        private readonly ReportService _reportService;

        // This is the actual constructor
        public ReportingServiceTests()
        {
            _analyticsRepoMock = new Mock<IAnalyticsRepository>();
            _reportService = new ReportService(_analyticsRepoMock.Object);
        }

        [Fact]
        public async Task GetOverviewReportAsync_ShouldReturnAggregatedValues_WhenDataExists()
        {
            var dailyStats = new List<DailyStats>
        {
            new DailyStats { TotalUsers = 100, TotalSessions = 200, TotalViews = 300, AvgPerformance = 0.9m },
            new DailyStats { TotalUsers = 150, TotalSessions = 250, TotalViews = 350, AvgPerformance = 0.8m }
        };
            _analyticsRepoMock.Setup(r => r.GetAggregatedStatsAsync()).ReturnsAsync(dailyStats);

            var result = await _reportService.GetOverviewReportAsync();

            Assert.True(result.IsSuccess);
            Assert.Equal(250, result.Data.TotalUsers);
            Assert.Equal(450, result.Data.TotalSessions);
            Assert.Equal(650, result.Data.TotalViews);
            Assert.Equal(0.85m, result.Data.AvgPerformance, 2);
        }

        [Fact]
    public async Task GetOverviewReportAsync_ShouldReturnZeros_WhenNoDataExists()
    {
        // Arrange
        _analyticsRepoMock.Setup(r => r.GetAggregatedStatsAsync())
                          .ReturnsAsync(new List<DailyStats>());

        // Act
        var result = await _reportService.GetOverviewReportAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.TotalUsers);
        Assert.Equal(0, result.Data.TotalSessions);
        Assert.Equal(0, result.Data.TotalViews);
        Assert.Equal(0, result.Data.AvgPerformance);
        Assert.Equal("NO_DATA", result.Code);
    }

    [Fact]
    public async Task GetPerPageReportAsync_ShouldReturnPageStats()
    {
        // Arrange
        var pageStats = new List<PageReportDto>
        {
            new PageReportDto { Page = "/home", TotalUsers = 100, TotalSessions = 200, TotalViews = 300, AvgPerformance = 0.9m },
            new PageReportDto { Page = "/about", TotalUsers = 50, TotalSessions = 100, TotalViews = 150, AvgPerformance = 0.8m }
        };
        _analyticsRepoMock.Setup(r => r.GetPerPageAggregatedStatsAsync())
                          .ReturnsAsync(pageStats);

        // Act
        var result = await _reportService.GetPerPageReportAsync();

        // Assert
        Assert.Equal(2, result.Count());
        var homePage = result.First(r => r.Page == "/home");
        Assert.Equal(100, homePage.TotalUsers);
        Assert.Equal(0.9m, homePage.AvgPerformance);
    }
    }
}
