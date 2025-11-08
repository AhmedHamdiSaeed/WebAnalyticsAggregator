using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text.Json;
using System.Threading.Tasks;

namespace Controllers
{
    [Route("api/[controller]")]
[ApiController]
public class testRabbitmqController : ControllerBase
{
        [HttpPost]
        public async Task<IActionResult> Post( [FromBody] string data)
        {

            var factory= new RabbitMQ.Client.ConnectionFactory() { 
                HostName = "rabbitmq" ,
                UserName="user",
                Password= "password",
                VirtualHost= "/"
            };
            var conn=await factory.CreateConnectionAsync();
            using var channel=await conn.CreateChannelAsync();   
           await channel.QueueDeclareAsync(queue: "testQueue",
                                 durable: true,
                                 exclusive: true,
                                 autoDelete: false,
                                 arguments: null);
            var jsonString=JsonSerializer.Serialize(data);
            var body=System.Text.Encoding.UTF8.GetBytes(jsonString);
            await channel.BasicPublishAsync(exchange: "",routingKey: "testQueue", body: body);
            return Ok("RabbitMQ is working!");
        }   
    }
}
