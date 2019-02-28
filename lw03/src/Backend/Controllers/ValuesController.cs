using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StackExchange.Redis;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();

        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            string value = null;
            _data.TryGetValue(id, out value);
            return value;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var id = Guid.NewGuid().ToString();
            try {
                StackExchange.Redis.ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
			    StackExchange.Redis.IDatabase redisDb = redis.GetDatabase();
                this.SaveDataToRedis(redisDb, id, value);
                this.makeEvent(redis, id, value);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return id;
        }

        private void SaveDataToRedis(StackExchange.Redis.IDatabase redisDb, string id, string value)
		{
			redisDb.StringSet(id, value);
            string savedData = redisDb.StringGet(id);
			Console.WriteLine(id + ": " + savedData + " - saved to redis");
		}
        
        private void makeEvent(StackExchange.Redis.ConnectionMultiplexer redis, String id, String value) {
            ISubscriber sub = redis.GetSubscriber();
            sub.Publish("events", id + " - " + value);
        }
    }
}
