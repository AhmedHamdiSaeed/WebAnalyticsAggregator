using DTOs.Reports;
using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.interfaces;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories
{


    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AnalyticsDbContext _dbContext;

        public AnalyticsRepository(AnalyticsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DailyStats>> GetAggregatedStatsAsync()
        {
            return await _dbContext.DailyStats.ToListAsync();
        }

        public async Task<IEnumerable<PageReportDto>> GetPerPageAggregatedStatsAsync()
        {
            return await _dbContext.RawData
                .GroupBy(r => r.Page)
                .Select(g => new PageReportDto
                {
                    Page = g.Key,
                    TotalUsers = g.Sum(x => x.Users),
                    TotalSessions = g.Sum(x => x.Sessions),
                    TotalViews = g.Sum(x => x.Views),
                    AvgPerformance = g.Average(x => x.PerformanceScore)
                })
                .ToListAsync();
        }

        public async Task SaveRawRecordAsync(CombinedRecord rawData)
        {
            _dbContext.RawData.Add(rawData);
            await _dbContext.SaveChangesAsync();
        }
    }
}
