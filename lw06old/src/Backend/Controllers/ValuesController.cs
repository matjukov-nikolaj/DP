using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Threading;
using Core;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private RedisDatabase redisDb = new RedisDatabase();
        private static Dictionary<string, string> properties = Configuration.GetParameters();
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();

        // GET api/values/<id>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
			IDatabase redisDb = redis.GetDatabase();
			for (short i = 0; i < 5; ++i)
			{
				string rank = redisDb.StringGet("RANK_" + id);
				if (rank == null)
				{
					Thread.Sleep(200);
				}
				else
				{
					return Ok(rank);
				}
			}
			return new NotFoundResult();
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var id = Guid.NewGuid().ToString();
            try {
                Message data = JsonConvert.DeserializeObject<Message>(value);
                IDatabase redisDb = this.redisDb.GetDatabase(data.GetLocation());
                string textKey = "TEXT_" + id;
                this.SaveDataToRedis(redisDbQ, textKey, value);
                //this.makeEvent(redisDbQ, textKey, value);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return id;
        }

        private void SaveDataToRedis(StackExchange.Redis.IDatabase redisDb, string id, string value)
		{
			redisDb.StringSet(id, value);
            string savedData = redisDb.StringGet(id);
			Console.WriteLine(id + ": " + value + " - saved to redis");
		}
        
        private void makeEvent(StackExchange.Redis.ConnectionMultiplexer redis, String id, String value) {
            ISubscriber sub = redis.GetSubscriber();
            sub.Publish("events", id);
        }
    }
}
