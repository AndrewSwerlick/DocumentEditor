using System;
using DocumentEditor.Commands.DTOs;
using DocumentEditor.Core;
using DocumentEditor.Core.Models;
using Microsoft.AspNet.SignalR;
using Raven.Client;

namespace DocumentEditor.Commands.DocumentCommands
{
    public class SubscribeToDocumentUpdatesCommand : ICommand
    {
        private readonly SubscriptionRequest _request;
        public IDocumentSession Session { get; set; }
     
        public SubscribeToDocumentUpdatesCommand(SubscriptionRequest request)
        {
            _request = request;
        }

        public void Execute()
        {
            var document = Session.Load<Document>(_request.Id);

            var subscriber = new Subscriber(_request.ConnectionId);
            document.AddSubscriber(subscriber);

            subscriber.SubscriberNotifiedOfUpdate +=
                (o, e) => _request.Connection.Send(_request.ConnectionId, e);
        }
    }
}
