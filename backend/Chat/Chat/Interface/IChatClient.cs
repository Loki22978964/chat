using Application.DTOs;

namespace Chat.Interface
{
    public interface IChatClient
    {
        Task ReceiveMessage(MessageDto message);
    }
}
