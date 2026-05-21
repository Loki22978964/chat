using Application.Common.Events;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chat.Controllers
{
    public record SendRequest(Guid ChatId, string Content);

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController(IChatService _chatService, AppDbContext _db, IPublishEndpoint _publishEndpoint) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetMyChats()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var chats = await _chatService.GetUserChatsAsync(userId); 
            return Ok(chats);
        }

        [HttpPost]
        public async Task<ActionResult<ChatDto>> CreateChat([FromBody] string chatName)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var chat = await _chatService.CreateChatAsync(chatName, userId);
            return Ok(chat);
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendRequest req)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

            var msg = new Domain.Entities.Message
            {
                Content = req.Content,
                ChatId = req.ChatId,
                UserId = userId
            };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            await _publishEndpoint.Publish(new MessageSentEvent
            {
                MessageId = msg.Id,
                ChatId = msg.ChatId,
                Content = msg.Content,
                UserName = userName,
                UserId = userId
            });

            return Ok(new { msg.Id, status = "Sent" });
        }

        //[HttpDelete("{chatId}")]
        //public async Task<IActionResult> DeleteChat(Guid chatId)
        //{
        //    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //    var success = await _chatService.DeleteChatAsync(chatId, userId);

        //    if (!success)
        //        return NotFound("Чат не знайдено або у вас немає прав на його видалення");

        //    return NoContent();
        //}

        [HttpPost("{chatId}/leave")]
        public async Task<IActionResult> LeaveChat(Guid chatId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _chatService.LeaveChatAsync(chatId, userId);

            return NoContent();
        }

        [HttpPost("{chatId}/join")]
        public async Task<IActionResult> JoinChat(Guid chatId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _chatService.JoinChatAsync(chatId, userId);
            return NoContent();
        }
    }
}
