﻿using System.Net;
using System.Net.Sockets;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using DocumentEditor.Core.Models;
using DocumentEditor.Web.Infrastructure;
using DocumentEditor.Web.Infrastructure.Serialization;
using DocumentEditor.Web.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using Ninject;
using Raven.Client;
using Raven.Client.Embedded;
using System;

namespace DocumentEditor.Web.Tests.Integration
{
    public class BaseIntegrationTestClass
    {
        protected EmbeddableDocumentStore DocumentStore { get; private set; }
        protected IDocumentSession Session { get; private set; }
        protected string Url { get; private set; }
        protected HttpServer Server { get; private set; }

        [TestFixtureSetUp]
        public void ClassSetup()
        {
            AutoMapper.Mapper.CreateMap<Document, DocumentData>();
        }

        [SetUp]
        public void Setup()
        {
            DocumentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true,
                    Conventions =
                        {
                            CustomizeJsonSerializer = MvcApplication.SetupSerializer
                        },
                };
          
            DocumentStore.Initialize();
            Session = DocumentStore.OpenSession();
            
            Url = "http://localhost:" + GetRandomUnusedPort();
            var config = new HttpSelfHostConfiguration(Url); 
            
            WebApiConfig.Register(config);
            config.Services.Replace(
                typeof (IHttpControllerActivator),
                new NinjectControllerActivator(BuildKernal(DocumentStore)));
            
            Server = new HttpServer(config);           
        }
      
        protected Document PopulateDatabaseWithSingleDocument(string contents)
        {
            var document = new Document(contents,Guid.NewGuid().ToString());
            using (Session)
            {
                Session.Store(document);
                Session.SaveChanges();
            }
            return document;
        }

        private static int GetRandomUnusedPort()
        {

            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
        private static IKernel BuildKernal(IDocumentStore store)
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<IDocumentStore>()
           .ToMethod(context => store.Initialize())
           .InSingletonScope();

            return kernel;
        }
    }
}
