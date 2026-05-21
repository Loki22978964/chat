using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _appDbContext;
        public MessageRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddAsync(Message message)
        {
            await _appDbContext.Messages.AddAsync(message);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, int limit = 50)
        {
            return await _appDbContext.Messages
                                      .Where(e => e.ChatId == chatId)
                                      .OrderByDescending(e => e.Timestamp)
                                      .Take(limit)
                                      .ToListAsync();
        }
    }
}
