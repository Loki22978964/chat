namespace Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatParticipant> Participants { get; set; }
    }
}
