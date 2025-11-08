using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.interfaces
{
    public interface IAnalyticsRepository
    {
        Task<IEnumerable<DailyStats>> GetAggregatedStatsAsync();
        Task<IEnumerable<DailyStats>> GetPerPageAggregatedStatsAsync();
        Task SaveRawRecordAsync(CombinedRecord rawData);
    }
}
