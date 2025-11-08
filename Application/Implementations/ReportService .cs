using Application.DTOs.Reports;
using Application.Interfaces;
using Infrastructure.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
   
        public class ReportService : IReportService
        {
            private readonly IAnalyticsRepository _analyticsRepository;

            public ReportService(IAnalyticsRepository analyticsRepository)
            {
                _analyticsRepository = analyticsRepository;
            }

            public async Task<OverviewReportDto> GetOverviewReportAsync()
            {
                var dailyStats = await _analyticsRepository.GetAggregatedStatsAsync();

                return new OverviewReportDto
                {
                    TotalUsers = dailyStats.Sum(x => x.TotalUsers),
                    TotalSessions = dailyStats.Sum(x => x.TotalSessions),
                    TotalViews = dailyStats.Sum(x => x.TotalViews),
                    AvgPerformance = dailyStats.Average(x => x.AvgPerformance)
                };
            }

            public async Task<IEnumerable<PageReportDto>> GetPerPageReportAsync()
            {
                var perPageStats = await _analyticsRepository.GetPerPageAggregatedStatsAsync();

                return perPageStats.Select(x => new PageReportDto
                {
                    TotalUsers = x.TotalUsers,
                    TotalSessions = x.TotalSessions,
                    TotalViews = x.TotalViews,
                    AvgPerformance = x.AvgPerformance
                });
            }
        }
}
