namespace API.Models
{
    /// <summary>
    /// Represents a log entry for a conversation message.
    /// </summary>
    public class Log
    {
        public string sessionID { get; set; }
        public string conversationID { get; set; }
        public string messageID { get; set; }
        public string userID { get; set; }



    }
    public class ChatHistoryItem
    {
        public ChatInput inputs { get; set; } = new ChatInput();
        public ChatOutput outputs { get; set; } = new ChatOutput();
    }
    public class ChatInput
    {
        public string question { get; set; }
    }
    public class ChatOutput
    {
        public string answer { get; set; }
        public string intent { get; set; }
        public string context { get; set; }

    }
}