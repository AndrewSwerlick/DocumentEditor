using DocumentEditor.Core;
using DocumentEditor.Core.Models;
using Raven.Client;

namespace DocumentEditor.Commands.DocumentCommands
{
    public class CreateDocumentCommand : ICommand
    {
        private readonly string _name;
        public IDocumentSession Session { get; set; }

        public CreateDocumentCommand(string name)
        {
            _name = name;
        }

        public void Execute()
        {
            var document = new Document();
            document.Name = _name;
            Session.Store(document);
        }
    }
}
