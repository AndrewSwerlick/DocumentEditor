using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocumentEditor.Web.Controllers
{
    public class TestsController : Controller
    {        
        public ActionResult Index()
        {
            ViewBag.TestFiles = BuildTestFilesList();
            ViewBag.SutFiles = BuildSystemUnderTestFilesList();
            return View();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
#if !DEBUG
            filterContext.Result = HttpNotFound();
#endif
            base.OnActionExecuted(filterContext);
        }

        private IList<string> BuildTestFilesList()
        {
            var scriptsDirectory = Server.MapPath("~/Scripts/spec/");
            return
                Directory.GetFiles(scriptsDirectory, "*.js", SearchOption.AllDirectories)
                .Select(s => s.Replace(scriptsDirectory, "/Scripts/spec/"))                
                .Select(s=> s.Replace("\\","/"))
                .Where(s=> !s.Contains("/lib/"))         
                         .ToList();
        } 

        private IList<string> BuildSystemUnderTestFilesList()
        {
            return BuildTestFilesList().Select(s => s.Replace("_spec", "")).ToList();

        } 
    }
}
