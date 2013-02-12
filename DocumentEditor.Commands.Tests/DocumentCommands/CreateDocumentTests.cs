using DocumentEditor.Commands.DocumentCommands;
using DocumentEditor.Core.Models;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Linq;

namespace DocumentEditor.Commands.Tests.DocumentCommands
{
    public class CreateDocumentTests : BaseTestClass
    {      
        [Test]
        public void Ensure_We_Can_Create_A_New_CreateDocumentCommand()
        {
            var command = new CreateDocumentCommand("Test");

            Assert.That(command, Is.Not.Null);
        }

        [Test]
        public void Ensure_We_Can_Execute_A_New_CreateDocumentCommand()
        {
            var command = new CreateDocumentCommand("Test");

            ExecuteCommand(command);
        }

        [Test]
        public void Ensure_When_We_Execute_A_CreateDocumentCommand_A_New_Document_Is_Added_To_The_DocumentStore()
        {
            var command = new CreateDocumentCommand("Test");
            ExecuteCommand(command);

            var newDoc = Session.Query<Document>().Where(d => d.Name == "Test").Lazily().Value;
            Assert.That(newDoc, Is.Not.Null);
        }      
    }
}
