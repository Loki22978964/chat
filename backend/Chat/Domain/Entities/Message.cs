using Domain.Entities.Enums;

namespace Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; } 
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } 
        public MessageStatus Status {  get; set; }

       
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid ChatId { get; set; }
        public Chat? Chat { get; set; }
    }
}
