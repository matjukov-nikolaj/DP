using System;

namespace Core
{
    public class Message
    {
        private string message;

        private string location;

        public Message(string message, string location) {
            this.message = message;
            this.location = location;
        }
        public string GetMessage() {
            return this.message;
        }

        public string GetLocation() {
            return this.location;
        }
    }
}