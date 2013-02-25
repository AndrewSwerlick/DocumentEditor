using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using AutoMapper;
using DocumentEditor.Commands.DTOs;
using DocumentEditor.Commands.DocumentCommands;
using DocumentEditor.Core.Models;
using DocumentEditor.Web.Infrastructure.Serialization;
using DocumentEditor.Web.Models;
using Newtonsoft.Json;
using Raven.Client;

namespace DocumentEditor.Web.Controllers
{
    public class DocumentsController : DataApiController
    {
        public DocumentsController(IDocumentStore documentStore) : base(documentStore)
        {
            
        }

        public DocumentData Get(string id)
        {
            var document = DocSession.Load<Document>(id);
            return Mapper.Map<Document, DocumentData>(document);
        }
        
        public bool Put(string id, HttpRequestMessage request)
        {
            var content = request.Content.ReadAsStringAsync().Result;
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new DiffConverter());
            var editReq= serializer.Deserialize<DocumentEditRequest>(new JsonTextReader(new StringReader(content)));
            var command = new AddRevisionToDocumentCommand(editReq) { Session = DocSession };
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
