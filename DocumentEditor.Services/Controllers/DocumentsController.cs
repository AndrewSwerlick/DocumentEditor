using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DocumentEditor.Commands;
using DocumentEditor.Commands.DTOs;
using DocumentEditor.Commands.DocumentCommands;

namespace DocumentEditor.Services.Controllers
{
    public class DocumentsController : ApiController
    {
        public void Edit(RevisionDTO revision)
        {
            var command = new AddRevisionToDocumentCommand(revision);
        }
    
        private void ExcecuteCommand(ICommand command)
        {
            
        } 
    }    
}
