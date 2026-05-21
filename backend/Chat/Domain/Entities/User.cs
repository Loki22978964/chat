using Domain.Entities.Enums;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public ICollection<ChatParticipant> Participants { get; set; }
        public ICollection<Message> Messages { get; set; }


    }
}