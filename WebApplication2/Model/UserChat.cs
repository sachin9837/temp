namespace API.Models
{
    /// <summary>
    /// Represents a user chat session, including chat history, session and conversation IDs, message and exception details, and timezone information.
    /// </summary>
    public class UserChat
    {
        public ChatTurn[] chatHistory { get; set; }
        public string question { get; set; }
        public string followUpFlag { get; set; }
        public string sessionID { get; set; }
        public string conversationID { get; set; }
        public string messageID { get; set; }
        public string logType { get; set; }
        public string timezone { get; set; }
        public string exception { get; set; }
        public string message { get; set; }
    }

    /// <summary>
    /// Represents a single turn in a chat conversation, containing information about the user, bot, intent, and context.
    /// </summary>
    public class ChatTurn
    {
        public string? user { get; set; }
        public string? bot { get; set; }
        public string? intent { get; set; }
        public string? context { get; set; }
    }
}