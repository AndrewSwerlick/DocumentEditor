using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace DocumentEditor.Web.Infrastructure
{
    public class DocumentConnection : PersistentConnection
    {
        private readonly SubscriptionManager _subscriptionManager;

        public DocumentConnection(SubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager;
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            _subscriptionManager.RegisterSubscription(data,connectionId, Connection);
            return Connection.Send(connectionId, "subscribed to updates for document " + data);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId)
        {
            _subscriptionManager.UnregisterSubscription(connectionId);
            return base.OnDisconnected(request, connectionId);
        }
    }
}