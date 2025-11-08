using Producer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.Services
{
    public interface IMessagePublisher
    {
        Task PublishAsync(CombinedRecord record);
        Task InitializeAsync();


    }
}
