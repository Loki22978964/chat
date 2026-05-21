using Domain.Entities.Enums;

namespace Domain.Entities
{
    public class GroupMessageStatus
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MessageId { get; set; }
        public Guid RecipientId { get; set; }
        public MessageStatus Status { get; set; } = MessageStatus.Pending; 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
