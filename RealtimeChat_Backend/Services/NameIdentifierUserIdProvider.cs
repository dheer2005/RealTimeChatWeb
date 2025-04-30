using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace RealtimeChat.Services
{
    public class NameIdentifierUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // Return the NameIdentifier claim (usually the username or user ID)
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
