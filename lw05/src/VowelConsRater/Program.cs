using System;
using StackExchange.Redis;

namespace VowelConsRaterc
{
    class Program
    {
        const string RATE_HINTS_CHANNEL = "rate_hints";
        const string RATE_QUEUE_NAME = "rate_queue";
        static void Main(string[] args)
        {
            Console.WriteLine("Vowel Cons Rater is running.");
            try {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase redisDb = redis.GetDatabase();
                ISubscriber sub = redis.GetSubscriber();
                sub.Subscribe(RATE_HINTS_CHANNEL, delegate
                {
                    // process all messages in queue
                    string msg = redisDb.ListRightPop(RATE_QUEUE_NAME);
                    while (msg != null)
                    {
                        string id = ParseData(msg, 0);

                        string vowels = ParseData(msg, 1);
                        string consonants = ParseData(msg, 2);
                        string result = vowels + "\\" + consonants;

                        string rankId = "RANK_" + id.Substring(5, id.Length - 5);
                        redisDb.StringSet(rankId, result);
                        Console.WriteLine(rankId + ": " + result + " - saved to redis");
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
