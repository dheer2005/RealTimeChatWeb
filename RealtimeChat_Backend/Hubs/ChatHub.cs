using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Context;
using RealtimeChat.Interfaces;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace RealtimeChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _dbContext;

        public ChatHub(ChatDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public override Task OnConnectedAsync()
        {
            if (!Context.User.Identity.IsAuthenticated)
            {
                Console.WriteLine("❌ User not authenticated!");
            }
            else
            {
                Console.WriteLine($"✅ Connected user: {Context.User.Identity.Name}");
            }

            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string fromUser, string userTo, string message, DateTime created, string status)
        {
            Console.WriteLine($"Sending message from {fromUser} to {userTo}: {message}");
            // Send message to the receiver
            await Clients.User(fromUser).SendAsync("ReceiveMessage", fromUser, userTo, message, created, status);

            // Optionally send back to sender to update their UI
            await Clients.User(userTo).SendAsync("ReceiveMessage", fromUser, userTo,message, created, status);

        }


        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendMessageToGroup(string groupName, string fromUser, string message, DateTime created)
        {
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", groupName, fromUser, message, created);
        }


        


    }
}
