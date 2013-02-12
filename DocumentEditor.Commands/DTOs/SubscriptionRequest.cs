using Microsoft.AspNet.SignalR;

namespace DocumentEditor.Commands.DTOs
{
    public class SubscriptionRequest
    {
        public string Id { get; set; }
        public string ConnectionId { get; set; }
        public IConnection Connection { get; set; }
    }
}
