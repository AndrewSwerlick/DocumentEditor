using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using AutoMapper;
using DocumentEditor.Commands.DTOs;
using DocumentEditor.Commands.DocumentCommands;
using DocumentEditor.Core.Models;
using DocumentEditor.Web.Models;
using Raven.Client;

namespace DocumentEditor.Web.Controllers
{
    public class DocumentsController : DataApiController
    {
        public DocumentsController(IDocumentStore documentStore) : base(documentStore)
        {
            
        }

        public DocumentData Get(int id)
        {
            var document = DocSession.Load<Document>("documents/" + id);
            return Mapper.Map<Document, DocumentData>(document);
        }

        public bool Put(int id, DocumentEditRequest request)
        {
            var document = DocSession.Load<Document>("documents/" + id);
            var command = new AddRevisionToDocumentCommand(request){Session = DocSession};
            command.Execute();
            return true;
        }

        public bool Post(DocumentCreationRequest request)
        {
            var command = new CreateDocumentCommand(request.Name) {Session = DocSession};
            command.Execute();
            return true;
        }
    
    }
}
