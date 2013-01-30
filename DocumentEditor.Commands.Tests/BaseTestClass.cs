using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;

namespace DocumentEditor.Commands.Tests
{
    public class BaseTestClass
    {
        public EmbeddableDocumentStore DocumentStore { get; set; }

        public IDocumentSession Session { get; set; }

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

        public void ExecuteCommand(ICommand command)
        {
            command.Session = Session;
            command.Execute();
        }
    }
}
