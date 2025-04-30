using Microsoft.Identity.Client;

namespace RealtimeChat.Models
{
    public class Messages
    {
        public int Id { get; set; }
        public string FromUser { get; set; } = string.Empty;
        public string UserTo { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public string Status { get; set; } = "sent";

    }
}
