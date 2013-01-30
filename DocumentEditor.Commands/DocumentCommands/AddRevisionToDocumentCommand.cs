using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentEditor.Commands.DTOs;
using DocumentEditor.Core;
using DocumentEditor.Core.Models;
using Raven.Client;

namespace DocumentEditor.Commands.DocumentCommands
{
    public class AddRevisionToDocumentCommand : ICommand
    {
        private readonly RevisionDTO _revisionDTO;
        public IDocumentSession Session { get; set; }

        public AddRevisionToDocumentCommand(RevisionDTO revision)
        {
            _revisionDTO = revision;
        }

        public void Execute()
        {
            var document = Session.Load<Document>(_revisionDTO.DocumentId);
            var parentRevision = document.LoadRevision(_revisionDTO.RevisionId);
            var revision = new BasicRevision(parentRevision, _revisionDTO.Patches);
            document.Edit(revision);
        }
    }
}
