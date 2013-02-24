using System;
using DocumentEditor.Core;
using DocumentEditor.Core.Models;
using Raven.Client;

namespace DocumentEditor.Commands.DocumentCommands
{
    public class CreateDocumentCommand : ICommand
    {
        private readonly string _id;
        private readonly string _name;
        public string IdToBeAssigned { get; private set; }
        public IDocumentSession Session { get; set; }

        public CreateDocumentCommand(string name)
        {
            _name = name;
            _id = Guid.NewGuid().ToString();
        }

        public CreateDocumentCommand(string name, string id) : this(name)
        {
            _id = id;
        }

        public void Execute()
        {
            var document = new Document("",_id) {Name = _name};
            Session.Store(document);
            IdToBeAssigned = document.Id;
        }
    }
}
