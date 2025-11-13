using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Producer.Services
{
    public class GADataReader : IDataReader<GARecord>
    {
        public async Task<List<GARecord>> ReadAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<List<GARecord>>(json) ?? new List<GARecord>();
        }
    }
}
