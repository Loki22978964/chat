using Application.Common.Events;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chat.Service
{
    public class ChatService(IChatRepository chatRepository, IPublishEndpoint _publishEndpoint) : IChatService
    {
        public async Task<ChatDto> CreateChatAsync(string chatName, Guid creatorId)
        {
            var chat = new Domain.Entities.Chat
            {
                Id = Guid.NewGuid(),
                Name = chatName,
                
                Participants = new List<Domain.Entities.ChatParticipant>
                {
                    new Domain.Entities.ChatParticipant
                    {
                        UserId = creatorId,
                        Role = Domain.Entities.Enums.ParticipantRole.Owner
                    }
                }
            };

            await chatRepository.CreateChatAsync(chat);

            return new ChatDto
            {
                Id = chat.Id,
                Name = chat.Name
            };
        }

        public async Task LeaveChatAsync(Guid chatId, Guid userId)
        {
            var participants = await chatRepository.GetParticipantAsync(chatId, userId);
            var participant = participants.FirstOrDefault();

            if (participant == null)
            {
                throw new InvalidOperationException("User is not a participant of this chat");
            }

            await chatRepository.RemoveParticipantAsync(participant);

            var remaining = await chatRepository.GetParticipantsByChatIdAsync(chatId);

            if (remaining.Any())
            {
                await chatRepository.DeleteChatAsync(chatId);
            }
        }

        public async Task<IEnumerable<ChatDto>> GetUserChatsAsync(Guid userId)
        {
            var chats = await chatRepository.GetChatsByUserIdAsync(userId);

            return chats.Select(chat => new ChatDto
            {
                Id = chat.Id,
                Name = chat.Name,
            });
        }

        public async Task JoinChatAsync(Guid chatId, Guid userId)
        {
            // 1. ПЕРЕВІРКА (Твій челендж)
            // Використай свій метод GetParticipantAsync, щоб дізнатися, чи юзер уже там.
            // Якщо список не порожній — кидай Exception, щоб не було дублікатів.

            var chatParticipant = await chatRepository.GetParticipantAsync(chatId, userId);

            if (chatParticipant.Any())
            {
                throw new InvalidOperationException("Ви вже є учасником цього чату");
            }

            // 2. СТВОРЕННЯ УЧАСНИКА
            var newParticipant = new Domain.Entities.ChatParticipant
            {
                ChatId = chatId,
                UserId = userId,
                Role = Domain.Entities.Enums.ParticipantRole.Member // Новачок — не власник
            };

            await chatRepository.AddParticipantAsync(newParticipant);

            // 3. ПОВІДОМЛЕННЯ (Fan-out)
            await _publishEndpoint.Publish(new UserJoinedEvent
            {
                ChatId = chatId,
                UserId = userId,
                UserName = "System",
                Timestamp = DateTime.UtcNow
            });
        }

    }
}
