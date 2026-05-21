using Domain.Entities;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _appDbContext;

        public ChatRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddParticipantAsync(ChatParticipant participant)
        {
            await _appDbContext.Participants.AddAsync(participant);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task CreateChatAsync(Chat chat)
        {
            await _appDbContext.Chats.AddAsync(chat);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            var chat = await _appDbContext.Chats
                                          .Include(x => x.Participants)
                                          .FirstOrDefaultAsync(x => x.Id == chatId);

            if (chat != null && !chat.Participants.Any())
            {
                _appDbContext.Chats.Remove(chat);
                await _appDbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId)
        {
            return await _appDbContext.Chats
                                      .Where(x => x.Participants.Any(e => e.UserId == userId))
                                      .ToListAsync();
        }

        public async Task<IEnumerable<ChatParticipant>> GetParticipantAsync(Guid chatId, Guid userId)
        {
            return await _appDbContext.Participants
                                        .Where(p => p.UserId == userId && p.ChatId == chatId)
                                        .ToListAsync();
        }

        public async Task<IEnumerable<ChatParticipant>> GetParticipantsByChatIdAsync(Guid chatId)
        {
            return await _appDbContext.Participants
                                .Where(e => e.ChatId == chatId)
                                .ToListAsync();
        }

        public async Task RemoveParticipantAsync(ChatParticipant participant)
        {
            if(participant != null)
            {
                _appDbContext.Participants.Remove(participant);
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}
