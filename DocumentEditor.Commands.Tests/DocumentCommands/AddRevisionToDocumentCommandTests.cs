using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentEditor.Commands.DTOs;
using DocumentEditor.Commands.DocumentCommands;
using DocumentEditor.Core;
using DocumentEditor.Core.Models;
using DocumentEditor.Core.Tests.Util;
using NUnit.Framework;

namespace DocumentEditor.Commands.Tests.DocumentCommands
{
    public class AddRevisionToDocumentCommandTests : BaseTestClass
    {
        [Test]
        public void Ensure_When_Execute_An_AddRevisionToDocumentCommand_The_Document_Has_Revised_Contents()
        {
            var document = new Document("Test");
            Session.Store(document);

            var revisionDTO = new DocumentEditRequest
            {
                DocumentId = document.Id,
                ParentRevisionId = document.CurrentRevision.Id,
                RevisionId = Guid.NewGuid(),
                Patches = Patches.Make(document.Contents, "Test changed").ToArray()
            };
            var command = new AddRevisionToDocumentCommand(revisionDTO);

            ExecuteCommand(command);

            Assert.That(document.Contents, Is.EqualTo("Test changed"));
        }
    }
}
