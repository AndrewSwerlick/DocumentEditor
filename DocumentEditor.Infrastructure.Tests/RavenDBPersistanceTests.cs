using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentEditor.Core.Models;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace DocumentEditor.Infrastructure.Tests
{
    public class RavenDBPersistanceTests
    {
        public EmbeddableDocumentStore DocumentStore { get; set; }

        public IDocumentSession Session { get; set; }

        [SetUp]
        public void Setup()
        {
            DocumentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true,
                    UseEmbeddedHttpServer = true,
                    Configuration =
                        {
                            Port = 12345,
                        },
                };
            DocumentStore.Initialize();
        }


        [TearDown]
        public void TearDown()
        {
            DocumentStore.HttpServer.Dispose();
        }      
    }
}
