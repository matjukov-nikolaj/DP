using System;
using Core;
using System.Collections.Generic;
using StackExchange.Redis;

namespace Core
{
    public class RedisDatabase
    {
        private static Dictionary<string, string> properties = Configuration.GetParameters();

        private ConnectionMultiplexer redisConnection;
        //private IDatabase redisQueue;
        private IDatabase redisEU;
        private IDatabase redisRU;
        private IDatabase redisUS;

        public RedisDatabase() {
            try {
                this.redisConnection = ConnectionMultiplexer.Connect(properties["REDIS_SERVER"]);
                this.redisEU = this.redisConnection.GetDatabase(1);
                this.redisRU = this.redisConnection.GetDatabase(2);
                this.redisUS = this.redisConnection.GetDatabase(3);
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
            
        }
        public IDatabase GetDatabase(string contextId) {
            switch(contextId.ToLower())
            {
                case "eu": 
                    return redisEU;
                case "rus": 
                    return redisRU;
                case "usa": 
                    return redisUS;  
                default: 
                    throw new Exception("Unknown redis contextId: " + contextId);
            }
        }

    }
}
