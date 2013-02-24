using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentEditor.Commands.DocumentCommands;
using DocumentEditor.Core.Models;
using DocumentEditor.Web.Models;
using Raven.Client;

namespace DocumentEditor.Web.Controllers
{
    public class DocumentEditorController : DataController
    {

        public DocumentEditorController(IDocumentStore documentStore)
            : base(documentStore)
        {
            
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(string id)
        {
            var document = DocSession.Load<Document>(id);
            return View(document);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string name)
        {
            var command = new CreateDocumentCommand(name){Session = DocSession};
            command.Execute();
            return RedirectToAction("Edit", new {id = command.IdToBeAssigned});
        }

    }
}
