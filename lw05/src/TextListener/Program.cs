using System;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Text listener is running.");
            try {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase redisDb = redis.GetDatabase();
                ISubscriber sub = redis.GetSubscriber();
                sub.Subscribe("events", (channel, message) => {
                    if (message.ToString().Contains("TEXT_")) {
                        Console.WriteLine("TextCreated: " + message);
                    }
                });
                Console.ReadKey();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
