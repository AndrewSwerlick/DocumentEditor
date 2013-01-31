using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Raven.Client;

namespace DocumentEditor.Web.Controllers
{
    public class DataController : ApiController
    {
        private readonly IDocumentStore _documentStore;

        public IDocumentSession DocSession { get; set; }

        public DataController(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public override async Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext,CancellationToken cancellationToken)
        {
            using (DocSession = _documentStore.OpenSession())
            {
                var result = await base.ExecuteAsync(controllerContext, cancellationToken);
                DocSession.SaveChanges();

                return result;
            }
        }
    }
}