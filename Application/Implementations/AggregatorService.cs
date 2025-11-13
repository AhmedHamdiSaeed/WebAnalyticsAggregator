using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Interfaces;
using DTOs;

namespace Application.Implementations
{
    public class AggregatorService: IAggregatorService
    {
      public IEnumerable<CombinedRecord>  AggregateAsyn(List<GARecord> gARecords,List<PSIRecord> pSIRecords)
        {
            return from ga in gARecords
                   join psi in pSIRecords
                   on new { ga.date, ga.page } equals new { psi.date, psi.page }
                   select new CombinedRecord
                   {
                       Date = ga.date,
                       Page = ga.page,
                       Users = ga.users,
                       Sessions = ga.sessions,
                       Views = ga.views,
                       PerformanceScore = psi.performanceScore,
                       LCPms = psi.LCP_ms
                   };
        }
    }
}
