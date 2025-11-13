
using Application.Interfaces;
using DTOs.Reports;
using DTOs;
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

            public async Task<Result<OverviewReportDto>> GetOverviewReportAsync()
            {
                var dailyStats = await _analyticsRepository.GetAggregatedStatsAsync();
            if (dailyStats.Any())
            {

                var OverviewReportDto = new OverviewReportDto
                {
                    TotalUsers = dailyStats.Sum(x => x.TotalUsers),
                    TotalSessions = dailyStats.Sum(x => x.TotalSessions),
                    TotalViews = dailyStats.Sum(x => x.TotalViews),
                    AvgPerformance = dailyStats.Average(x => x.AvgPerformance)
                };
                return  Result<OverviewReportDto>.Success(OverviewReportDto);   

            }
            return Result<OverviewReportDto>.Success(
                new OverviewReportDto
                {
                    TotalUsers = 0,
                    TotalSessions = 0,
                    TotalViews = 0,
                    AvgPerformance = 0
                },
                code: "NO_DATA");
        }

            public async Task<IEnumerable<PageReportDto>> GetPerPageReportAsync()
            {
               return await _analyticsRepository.GetPerPageAggregatedStatsAsync();
            }
        }
}
