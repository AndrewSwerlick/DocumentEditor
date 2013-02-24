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
using DocumentEditor.Web.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            var jSettings = new JsonSerializerSettings();
            jSettings.Converters.Add(new DiffConverter());
            jsonFormatter.SerializerSettings = jSettings;

            AutoMapper.Mapper.CreateMap<DocumentData, Document>();
        }
    }
}