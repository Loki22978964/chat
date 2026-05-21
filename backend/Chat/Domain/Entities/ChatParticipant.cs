using Domain.Entities.Enums;

namespace Domain.Entities
{
    public class ChatParticipant
    {
        public Guid UserId { get; set; }
        public ParticipantRole Role {  get; set; } = ParticipantRole.Member;
        public User? User { get; set; }

        public Guid ChatId { get; set; }
        public Chat? Chat { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
