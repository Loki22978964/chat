using Application.DTOs;
using Application.Interfaces;
using Domain.Entities.Enums;

namespace Application.Chat.Service
{
    public class MessageService(IMessageRepository messageRepository,
                                    IChatRepository chatRepository) : IMessageService
    {
        public async Task<IEnumerable<MessageDto>> GetChatHistoryAsync(Guid chatId, Guid userId, int limit = 50)
        {
            var chats = await chatRepository.GetChatsByUserIdAsync(userId);

            if(!chats.Any(e => e.Id == chatId))
            {
                throw new UnauthorizedAccessException("You are not a participant in this chat");
            }

            var messages = await messageRepository.GetMessagesByChatIdAsync(chatId, limit);

            return messages.Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                Timestamp = m.Timestamp,
                UserId = m.UserId,
                ChatId = m.ChatId,
                Status = m.Status
            });
        }

        public async Task<MessageDto> SendMessageAsync(Guid chatId, Guid userId, string content)
        {
            var message = new Domain.Entities.Message
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                UserId = userId,
                Content = content,
                Timestamp = DateTime.UtcNow,
                Status = MessageStatus.Sent
            };

            await messageRepository.AddAsync(message);

            return new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                Timestamp = message.Timestamp,
                UserId = message.UserId,
                ChatId = message.ChatId,
                Status = message.Status
            };
        }
    }
}
