using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record GARecord(DateTime date, string page, int users, int sessions, int views);

}
