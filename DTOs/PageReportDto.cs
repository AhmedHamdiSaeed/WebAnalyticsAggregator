using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dtos
{
    public class PageReportDto
    {
        public string Page { get; set; } = null!;
        public int TotalUsers { get; set; }
        public int TotalSessions { get; set; }
        public int TotalViews { get; set; }
        public decimal AvgPerformance { get; set; }
    }
}
