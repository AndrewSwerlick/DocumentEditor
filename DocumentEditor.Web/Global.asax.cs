using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using DocumentEditor.Core.Models;
using DocumentEditor.Web.Infrastructure.Serialization;
using DocumentEditor.Web.Models;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Imports.Newtonsoft.Json;

namespace DocumentEditor.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
      
        public static void SetupSerializer(JsonSerializer serializer)
        {
            serializer.ContractResolver = new FluentContractResolver().MarkAsReference<IRevision>();
            serializer.Converters.Add(new SubscriberConverter());
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AutoMapper.Mapper.CreateMap<DocumentData, Document>();
        }
    }
}