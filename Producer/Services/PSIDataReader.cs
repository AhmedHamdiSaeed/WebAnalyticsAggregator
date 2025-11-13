using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Producer.Services
{
    public class PSIDataReader : IDataReader<PSIRecord>
    {
        public async Task<List<PSIRecord>> ReadAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<PSIRecord>>(json) ?? new List<PSIRecord>();
        }
    }
}
