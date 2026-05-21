//using Application.DTOs;
//using Application.Interfaces;
//using Chat.Models;
//using Domain.Entities;
//using Infrastructure;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Caching.Distributed;
//using System.Text.Json;

//namespace Chat.Hubs
//{
//    public interface IChatClient
//    {
//        public Task ReceiveMessage(MessageDto message);
//    }
//                     //IDistributedCache cache,
//                     //   |
//                     //   V
//    public class ChatHub(IMessageService messageService) : Hub<IChatClient>
//    {
//        public async Task JoinChat(Guid chatId)
//        {
//            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

//            var messageDto = new MessageDto
//            {
//                Id = Guid.NewGuid(),
//                UserName = "System",
//                Content = $"User {Context.UserIdentifier} joined the chat",
//                Timestamp = DateTime.UtcNow,
//                UserId = Guid.Empty,
//                ChatId = chatId
//            };


//            await Clients
//                .Group(chatId.ToString())
//                .ReceiveMessage(messageDto);
//        }

//        public async Task SendMessage(Guid chatId, string content)
//        {
//            var userIdString = Context.UserIdentifier;

//            if(string.IsNullOrEmpty(userIdString))
//            {
//                return;
//            }

//            var messageDto = await messageService.SendMessageAsync(chatId, Guid.Parse(userIdString), content);

//            await Clients.Group(chatId.ToString()).ReceiveMessage(messageDto);
//        }

//        public override async Task OnDisconnectedAsync(Exception? exception)
//        {
//            await base.OnDisconnectedAsync(exception);
//        }

//        public async Task LeaveChat(Guid chatId)
//        {
//            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
//        }
//    }
//}


using Chat.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Chat.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task JoinChat(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task LeaveChat(Guid chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}