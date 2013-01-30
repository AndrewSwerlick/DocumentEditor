using System.Linq;
using DocumentEditor.Commands.DTOs;
using DocumentEditor.Commands.DocumentCommands;
using DocumentEditor.Core.Models;
using DocumentEditor.Tests.Core.Util;
using Microsoft.AspNet.SignalR;
using NSubstitute;
using NUnit.Framework;

namespace DocumentEditor.Commands.Tests.DocumentCommands
{
    class SubscribeToDocumentUpdatesCommandTests : BaseTestClass
    {
        [Test]
        public void
            Ensure_That_When_We_Execute_A_SubscribeToDocumentUpdatesCommand_The_Documents_Subscriber_Property_Contains_A_New_Subscriber
            ()
        {
            var document = new Document("Test");
            Session.Store(document);
            var request = new SubscriptionRequest
                {
                    Id = document.Id
                };

            var command = new SubscribeToDocumentUpdatesCommand(request);
            ExecuteCommand(command);

            Assert.That(document.Subcribers.Count(), Is.EqualTo(1));
        }

        [Test]
        public void
            Given_That_We_Have_Executed_The_SubscribeToDocumentUpdatesCommand_Ensure_That_When_The_Document_Is_Edited_The_Connection_Associated_With_The_Request_Has_Its_Send_Method_Called()
        {
            var document = new Document("Test");
            Session.Store(document);
            var connection = Substitute.For<IConnection>();

            var request = new SubscriptionRequest
                {
                    Id = document.Id,
                    Connection = connection
                };

            var command = new SubscribeToDocumentUpdatesCommand(request);
            ExecuteCommand(command);

            document.Edit(new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, "Test2")));

            connection.Received().Send(Arg.Any<ConnectionMessage>());
        }
    }
}
