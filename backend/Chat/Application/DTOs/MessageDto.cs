using Domain.Entities.Enums;

namespace Application.DTOs
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public MessageStatus Status { get; set; }
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
    }
}
