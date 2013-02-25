using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using DocumentEditor.Commands.DTOs;
using Microsoft.AspNet.SignalR;
using DocumentEditor.Commands.DocumentCommands;

namespace DocumentEditor.Web.Infrastructure
{
    public class SubscriptionManager
    {
        private readonly IDictionary<string, string> _connectionToDocumentsMap = new Dictionary<string, string>();
        private readonly IDocumentStore _store;

        public SubscriptionManager(IDocumentStore store)
        {
            _store = store;
        }

        public void RegisterSubscription(string docId, string clientId, IConnection connection)
        {
            var subscriptionRequest = new SubscriptionRequest
            {
                ConnectionId = clientId,
                Connection = connection,
                Id = docId
            };
            var command = new SubscribeToDocumentUpdatesCommand(subscriptionRequest);
            using (var session = _store.OpenSession())
            {
                command.Session = session;
                command.Execute();
                session.SaveChanges();
            }
            _connectionToDocumentsMap.Add(clientId,docId);
        }
        public void UnregisterSubscription(string clientId)
        {
            if (_connectionToDocumentsMap.ContainsKey(clientId))
            {
                var command = new UnsubscribeFromDocumentUpdatesCommand(clientId,
                                                                        _connectionToDocumentsMap[clientId]);
                using (var session = _store.OpenSession())
                {
                    command.Session = session;
                    command.Execute();
                    session.SaveChanges();
                }
            }
        }
    }
}