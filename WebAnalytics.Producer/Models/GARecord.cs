using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.Models
{
    public class GARecord
    {
        public string Date { get; set; } = "";
        public string Page { get; set; } = "";
        public int Users { get; set; }
        public int Sessions { get; set; }
        public int Views { get; set; }
    }
}
