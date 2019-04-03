using System;
using System.Collections.Generic;

namespace Core
{
    public class Message
    {
        private string message;

        private string location;

        private string id;

        private int database;

        public Message(string message, string location)
        {
            this.message = message;
            this.location = location;
            this.id = "";
            this.database = GetDatabaseNumber(location);
        }

        public string GetMessage()
        {
            return this.message;
        }

        public string GetLocation()
        {
            return this.location;
        }

        public string GetId()
        {
            return this.id;
        }

        public void SetID(string id)
        {
            this.id = id;
        }

        public int GetDatabase()
        {
            return this.database;
        }

        public static int GetDatabaseNumber(string contextId)
        {
            switch (contextId.ToLower())
            {
                case "eu":
                    return 1;
                case "ru":
                    return 2;
                case "us":
                    return 3;
                default:
                    throw new Exception("Unknown redis contextId: " + contextId);
            }
        }
    }
}