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
        private readonly DocumentEditRequest _documentEditRequest;
        public IDocumentSession Session { get; set; }

        public AddRevisionToDocumentCommand(DocumentEditRequest revision)
        {
            _documentEditRequest = revision;
        }

        public void Execute()
        {
            var document = Session.Load<Document>(_documentEditRequest.DocumentId);
            var parentRevision = document.LoadRevision(_documentEditRequest.RevisionId);
            var revision = new BasicRevision(parentRevision, _documentEditRequest.Patches);
            document.Edit(revision);
        }
    }
}
