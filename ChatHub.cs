using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    public async Task SendMessage(string senderId, string receiverId, string message, string context)
    {
        await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message, context);
    }
}
