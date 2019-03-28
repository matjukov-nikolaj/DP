using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Threading;
using Core;
using Newtonsoft.Json;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static Dictionary<string, string> properties = Configuration.GetParameters();
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();

        [HttpGet("{rank}")]
        public IActionResult Get([FromQuery]string id, [FromQuery]string location)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(properties["REDIS_SERVER"]);
            IDatabase redisDb = redis.GetDatabase(Message.GetDatabaseNumber(location));
            for (short i = 0; i < 5; ++i)
            {
                string rank = redisDb.StringGet("RANK_" + id);
                if (rank == null)
                {
                    Thread.Sleep(200);
                }
                else
                {
                    return Ok("Rank=" + rank + " Location=" + location);
                }
            }

            return new NotFoundResult();
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody] string value)
        {
            var id = Guid.NewGuid().ToString();
            try
            {
                Message data = new Message(ParseData(value, 0), ParseData(value, 1));
                string textKey = "TEXT_" + id;
                data.SetID(textKey);
                this.SaveDataToRedis(data.GetDatabase(), data);
                this.makeEvent(ConnectionMultiplexer.Connect(properties["REDIS_SERVER"]), data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return id;
        }
        private void SaveDataToRedis(int dbNumber, Message message)
        {
            var redisDb = ConnectionMultiplexer.Connect(properties["REDIS_SERVER"]).GetDatabase(dbNumber);
            redisDb.StringSet(message.GetId(), message.GetMessage());
            string savedData = redisDb.StringGet(message.GetId());
            Console.WriteLine(message.GetId() + ": " + message.GetMessage() + " - saved to redis " + message.GetLocation() + " : " + message.GetDatabase());
        }
        private void makeEvent(ConnectionMultiplexer redis, Message data)
        {
            ISubscriber sub = redis.GetSubscriber();
            sub.Publish("events", $"{data.GetId()}:{data.GetMessage()}:{data.GetLocation()}");
        }
        
        private string ParseData(string msg, int index)
        {
            return msg.Split(':')[index];
        }
    }
}