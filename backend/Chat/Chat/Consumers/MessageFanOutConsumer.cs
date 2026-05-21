namespace Chat.Consumers
{
    using Application.Common.Events;
    using Application.DTOs;
    using Chat.Hubs;
    using Chat.Interface;
    using MassTransit;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using StackExchange.Redis;

    public class MessageFanOutConsumer(
        Infrastructure.Persistence.AppDbContext _db,
        IConnectionMultiplexer _redisMultiplexer,
        IHubContext<ChatHub, IChatClient> _hubContext) : IConsumer<MessageSentEvent>
    {
        public async Task Consume(ConsumeContext<MessageSentEvent> context)
        {
            var msg = context.Message;
            var redis = _redisMultiplexer.GetDatabase();

            // 1. Отримуємо список учасників
            var participants = await _db.Participants
                .Where(p => p.ChatId == msg.ChatId)
                .ToListAsync();

            // 2. Створюємо статуси в Redis (Fan-out on Write)
            foreach (var p in participants)
            {
                string key = $"status:msg:{msg.MessageId}:user:{p.UserId}";
                await redis.StringSetAsync(key, "Delivered", TimeSpan.FromDays(7));
            }

            // 3. Відправляємо через SignalR (Push Service)
            await _hubContext.Clients.Group(msg.ChatId.ToString()).ReceiveMessage(new MessageDto
            {
                Id = msg.MessageId,
                Content = msg.Content,
                UserName = msg.UserName,
                ChatId = msg.ChatId,
                UserId = msg.UserId,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
