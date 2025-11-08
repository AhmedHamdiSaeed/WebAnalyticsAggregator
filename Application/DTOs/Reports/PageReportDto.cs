using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reports
{
    public class PageReportDto
    {
        public string Page { get; set; } = "";
        public int TotalUsers { get; set; }
        public int TotalSessions { get; set; }
        public int TotalViews { get; set; }
        public double AvgPerformance { get; set; }
    }
}
