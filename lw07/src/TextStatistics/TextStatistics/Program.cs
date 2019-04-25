using System;
using System.Collections.Generic;
using Core;
using StackExchange.Redis;

namespace TextStatistics
{
    class Program
    {
        
        private static Dictionary<string, string> properties = Configuration.GetParameters();

        private const string RANK_CALCULATED_CHANNEL = "text_rank_calculated";

        private static int textNum = 0;

        private static int highRankPart = 0;

        private static double avgRank = 0;

        private static double result = 0;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Text statistic is running.");
            try {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(properties["REDIS_SERVER"]);
                ISubscriber sub = redis.GetSubscriber();
                //Переделать канал
                sub.Subscribe("events", (channel, message) =>
                {
                    string value = ParseData(message, 1);
                    string[] values = value.Split('\\');
                    double convertedResult = values[1] == "0"
                        ? Convert.ToDouble(values[0])
                        : Convert.ToDouble(values[0]) / Convert.ToDouble(values[1]);
                    result += convertedResult;

                    ++textNum;
                    if (convertedResult > 0.5)
                    {
                        ++highRankPart;
                    }

                    avgRank = result / textNum;

                    string resultMessage = "TextNum: " + textNum + ", HighRankPart: " + highRankPart + ", AvgRank: " +
                                           avgRank;

                    SaveDataToRedis(redis, resultMessage);
                    
                    Console.WriteLine(resultMessage);

                });
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static void SaveDataToRedis(ConnectionMultiplexer redis, string data)
        {
            var redisDb = redis.GetDatabase(Convert.ToInt32(properties["QUEUE_DB"]));
            redisDb.StringSet("text_statistic", data);
        }

        private static string ParseData(string msg, int index)
        {
            return msg.Split(':')[index];
        }
    }
}