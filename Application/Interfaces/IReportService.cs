using Application.DTOs.Reports;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReportService
    {
        Task<OverviewReportDto> GetOverviewReportAsync();
        Task<IEnumerable<PageReportDto>> GetPerPageReportAsync();
    }
}
