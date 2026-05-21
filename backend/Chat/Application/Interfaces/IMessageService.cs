using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDto>> GetChatHistoryAsync(Guid chatId, Guid userId, int limit = 50);
        Task<MessageDto> SendMessageAsync(Guid chatId, Guid userId, string content);
    }
}
