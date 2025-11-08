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

        public async Task<IEnumerable<DailyStats>> GetPerPageAggregatedStatsAsync()
        {
            // Example: group by Page (if you have RawData table)
            return await _dbContext.DailyStats.ToListAsync();
        }

        public async Task SaveRawRecordAsync(CombinedRecord rawData)
        {
            _dbContext.RawData.Add(rawData);
            await _dbContext.SaveChangesAsync();
        }
    }
}
