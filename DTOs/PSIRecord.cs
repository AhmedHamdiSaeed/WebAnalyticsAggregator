using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record PSIRecord(DateTime date, string page, decimal performanceScore, int LCP_ms);

}
