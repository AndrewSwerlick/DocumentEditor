using DocumentEditor.Core;
using DocumentEditor.Core.Models;
using Raven.Client;

namespace DocumentEditor.Commands.DocumentCommands
{
    public class CreateDocumentCommand : ICommand
    {
        private readonly string _name;
        public string IdToBeAssigned { get; private set; }
        public IDocumentSession Session { get; set; }

        public CreateDocumentCommand(string name)
        {
            _name = name;
            IdToBeAssigned = name;
        }

        public void Execute()
        {
            var document = new Document {Name = _name};
            Session.Store(document);
        }
    }
}
