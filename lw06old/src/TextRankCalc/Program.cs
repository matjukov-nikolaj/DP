using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
    {
        const string COUNTER_HINTS_CHANNEL = "counter_hints";
        const string COUNTER_QUEUE_NAME = "counter_queue";
        static void Main(string[] args)
        {
            try {
                Console.WriteLine("Text rank calculator is running.");
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase redisDb = redis.GetDatabase();
                ISubscriber sub = redis.GetSubscriber();
                sub.Subscribe("events", (channel, message) => {
                    string msg = message.ToString();
                    if (msg.Contains("TEXT_")) {
                        String value = GetTextById(redisDb, msg);
                        SendMessage($"{msg}:{value}", redisDb);
                        Console.WriteLine("Message sent => " + msg + ": " + value);                        
                    }
                });
                Console.ReadKey();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        
        private static void SendMessage(string message, IDatabase db )
        {
            // put message to queue
            db.ListLeftPush( COUNTER_QUEUE_NAME, message, flags: CommandFlags.FireAndForget );
            // and notify consumers
            db.Multiplexer.GetSubscriber().Publish( COUNTER_HINTS_CHANNEL, "" );
        }

        private static string GetTextById(IDatabase db, string id) {
            string savedData = db.StringGet(id);
            return savedData;
        }
    }
}
