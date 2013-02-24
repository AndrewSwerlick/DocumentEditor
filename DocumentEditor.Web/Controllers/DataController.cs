using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Raven.Client;

namespace DocumentEditor.Web.Controllers
{
    public class DataController : Controller
    {
        private readonly IDocumentStore _documentStore;

        protected IDocumentSession DocSession { get; private set; }

        public DataController(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DocSession = _documentStore.OpenSession();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            DocSession.SaveChanges();
            DocSession.Dispose();
        }
    }
}
