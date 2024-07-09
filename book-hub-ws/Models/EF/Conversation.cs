using FirebaseAdmin.Messaging;

namespace book_hub_ws.Models.EF
{
    public class Conversation
    {
        public int Id { get; set; }
        public int InitiatorUserId { get; set; }
        public User InitiatorUser { get; set; }
        public int ReceiverUserId { get; set; }
        public User ReceiverUser { get; set; }
        public ICollection<Message> Messages { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public int SenderUserId { get; set; }
        public User SenderUser { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }


    public class StartConversationDto
    {
        public int InitiatorUserId { get; set; }
        public string ReceiverNickname { get; set; }
    }

    public class ConversationDto
    {
        public int Id { get; set; }
        public string InitiatorUserNickname { get; set; }
        public string ReceiverUserNickname { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastUpdated { get; set; }
        public int UnreadMessagesCount { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderUserId { get; set; }
        public string SenderNickname { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }
}
