using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
    {

        private static Set<char> VOWELS = new HashSet<char>
		{
			'a', 'e', 'i', 'o', 'u', 'y'
		};

        private static Set<char> CONSONANTS = new HashSet<char>
		{
			'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n',
			'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'
		};

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
                        HandleText(msg);
                    }
                });
                Console.ReadKey();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void HandleText(string id) {
            if (id == null || id == "") {
                Console.WriteLine("Empty message");
                return;
            }
            int vowels = 0;
			int consonants = 0;
            string text = GetTextById(id);
            foreach (char ch in text.ToLower()) {
                if (VOWELS.Contains(ch)) {
                    ++vowels;
                } else if (CONSONANTS.Contains(ch)) {
                    ++consonants;
                } else {
                    Console.WriteLine("Unknown character: " + ch);
                }
            }
            string result = vowels + "\\" + consonants;
            SaveDataToRedis(id, result);
        }

        private static string GetTextById(string id) {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
			IDatabase database = redis.GetDatabase();
            string savedData = database.StringGet(id);
            return savedData;
        }

        private static void SaveDataToRedis(string id, string value)
		{
			ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
			IDatabase database = redis.GetDatabase();
            string rankId = "RANK_" + id.Substring(5, id.Length - 5);
            database.StringSet(rankId, value);
			Console.WriteLine(id + ": " + value + " - saved to redis");
		}
    }
}