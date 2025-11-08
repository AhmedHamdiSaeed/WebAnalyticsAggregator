using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public  class CombinedRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Page { get; set; } = null!;
        public int Users { get; set; }
        public int Sessions { get; set; }
        public int Views { get; set; }
        public decimal PerformanceScore { get; set; }
        public int LCPms { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}
