using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using DocumentEditor.Core.Models;
using DocumentEditor.Web.Models;
using Raven.Client;

namespace DocumentEditor.Web.Controllers
{
    public class DocumentsController : DataController
    {
        public DocumentsController(IDocumentStore documentStore) : base(documentStore)
        {
            
        }

        public DocumentData Get(int id)
        {
            var document = DocSession.Load<Document>("documents/" + id);
            return Mapper.Map<Document, DocumentData>(document);
        }
    
    }
}
