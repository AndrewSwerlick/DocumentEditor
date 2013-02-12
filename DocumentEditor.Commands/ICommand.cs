using Raven.Client;

namespace DocumentEditor.Commands
{
    public interface ICommand
    {
        IDocumentSession Session { get; set; }

        void Execute();
    }
}
