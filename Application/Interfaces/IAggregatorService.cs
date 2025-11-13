using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;

namespace Application.Interfaces
{
    public interface IAggregatorService
    {
        IEnumerable<CombinedRecord> AggregateAsyn(List<GARecord> gARecords, List<PSIRecord> pSIRecords);
    }
}
