using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentEditor.Core.Models;
using Raven.Client;

namespace DocumentEditor.Commands.DocumentCommands
{
    public class UnsubscribeFromDocumentUpdatesCommand : ICommand
    {
        private readonly string _clientId;
        private readonly string _documentId;

        public UnsubscribeFromDocumentUpdatesCommand(string clientId, string documentId)
        {
            _clientId = clientId;
            _documentId = documentId;
        }

        public IDocumentSession Session { get; set; }
        public void Execute()
        {
            var document = Session.Load<Document>(_documentId);
            var subscriber = document.Subcribers.SingleOrDefault(s => s.Id == _clientId);
            if (subscriber != null)
                document.Subcribers.Remove(subscriber);
        }
    }
}
