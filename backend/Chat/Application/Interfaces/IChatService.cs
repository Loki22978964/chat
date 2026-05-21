using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatDto>> GetUserChatsAsync(Guid userId);
        Task<ChatDto> CreateChatAsync(string chatName, Guid creatorId);
        Task LeaveChatAsync(Guid chatId, Guid userId);
        Task JoinChatAsync(Guid chatId, Guid userId);
    }
}
