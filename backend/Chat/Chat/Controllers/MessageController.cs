using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chat.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController(IMessageService messageService) : ControllerBase
    {
        [HttpGet("{chatId}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetHistory(Guid chatId, [FromQuery] int limit = 50)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var messages = await messageService.GetChatHistoryAsync(chatId, userId, limit);

            return Ok(messages);
        }
    }
}
