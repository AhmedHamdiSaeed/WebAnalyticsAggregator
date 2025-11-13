using DTOs.Reports;
using DTOs;
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
        Task<Result<OverviewReportDto>> GetOverviewReportAsync();
        Task<IEnumerable<PageReportDto>> GetPerPageReportAsync();
    }
}
