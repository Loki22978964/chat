using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, int limit = 50);

        Task AddAsync(Message message);
    }
}
