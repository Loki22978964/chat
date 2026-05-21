using Domain.Entities;

namespace Application.Interfaces
{
    public interface IChatRepository
    {
        Task CreateChatAsync(Domain.Entities.Chat chat);
        Task DeleteChatAsync(Guid chatId);
        Task<IEnumerable<Domain.Entities.Chat>> GetChatsByUserIdAsync(Guid userId);
        Task<IEnumerable<ChatParticipant>> GetParticipantAsync(Guid chatId, Guid userId);
        Task<IEnumerable<ChatParticipant>> GetParticipantsByChatIdAsync(Guid chatId);
        Task RemoveParticipantAsync(ChatParticipant participant);
        Task AddParticipantAsync(ChatParticipant participant);
    }
}
