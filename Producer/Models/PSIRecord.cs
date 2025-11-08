using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.Models
{
    public record PSIRecord(string Date, string Page, double PerformanceScore, int LCP_ms);

}
