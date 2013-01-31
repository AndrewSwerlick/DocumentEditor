using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using DocumentEditor.Core.Models;
using DocumentEditor.Web.Models;
using Raven.Client.Document;

namespace DocumentEditor.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.Services.Replace(
                typeof (IHttpControllerActivator),
                new NinjectControllerActivator(NinjectKernelBuilder.BuildKernal()));

            AutoMapper.Mapper.CreateMap<DocumentData, Document>();
        }
    }
}