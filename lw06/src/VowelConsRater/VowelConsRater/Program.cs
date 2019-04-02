using System;
using System.Collections.Generic;
using Core;
using StackExchange.Redis;

namespace VowelConsRaterc
{
    class Program
    {
        private static Dictionary<string, string> properties = Configuration.GetParameters();
        
        const string RATE_HINTS_CHANNEL = "rate_hints";
        const string RATE_QUEUE_NAME = "rate_queue";
        static void Main(string[] args)
        {
            Console.WriteLine("Vowel Cons Rater is running.");
            try {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(properties["REDIS_SERVER"]);
                ISubscriber sub = redis.GetSubscriber();
                sub.Subscribe(RATE_HINTS_CHANNEL, delegate
                {
                    // process all messages in queue
                    IDatabase redisDbQueue = redis.GetDatabase(Convert.ToInt32(properties["QUEUE_DB"]));
                    string msg = redisDbQueue.ListRightPop(RATE_QUEUE_NAME);
                    while (msg != null)
                    {
                        string id = ParseData(msg, 0);

                        string result = ParseData(msg, 1);
                        string location = redisDbQueue.StringGet(id);
                        Message data = new Message(result, location);

                        string rankId = "RANK_" + id.Substring(5, id.Length - 5);
                        IDatabase redisDb = redis.GetDatabase(Convert.ToInt32(data.GetDatabase()));
                        redisDb.StringSet(rankId, result);
                        Console.WriteLine(rankId + ": " + result + " - saved to redis. Database: " + data.GetDatabase() + " - " + location);
                        msg = redisDb.ListRightPop(RATE_QUEUE_NAME);
                    }
                });
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static string ParseData(string msg, int index)
        {
            return msg.Split(':')[index];
        }
    }
}
