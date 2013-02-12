using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;

namespace DocumentEditor.Commands.Tests
{
    public class BaseTestClass
    {
        private EmbeddableDocumentStore DocumentStore { get; set; }

        protected IDocumentSession Session { get; private set; }

        [SetUp]
        public void Setup()
        {
            DocumentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true
                };
            DocumentStore.Initialize();
            Session = DocumentStore.OpenSession();
        }

        [TearDown] 
        public void TearDown()
        {
            Session.Dispose();
        }

        protected void ExecuteCommand(ICommand command)
        {
            command.Session = Session;
            command.Execute();
        }
    }
}
