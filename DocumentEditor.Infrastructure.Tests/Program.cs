using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentEditor.Core.Models;
using DocumentEditor.Core.Tests.Util;
using DocumentEditor.Infrastrcture.Serialization;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace DocumentEditor.Infrastructure.Tests
{
    public static class Program
    {
        public static void Main(string[] arts)
        {
            var DocumentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true,
                    UseEmbeddedHttpServer = true,
                    Configuration =
                        {
                            Port = 12345,
                        },
                };
            
            DocumentStore.Initialize();

            Document document = null;
            using (var session = DocumentStore.OpenSession())
            {
                document = new Document("Test");
                var initialRevision = document.CurrentRevision;
                var revision = new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, "Test2"));
                document.Edit(revision);
                session.Store(document);
                
                session.SaveChanges();
            }
            Console.ReadLine();

            using (var session = DocumentStore.OpenSession())
            {
                var loadedDoc = session.Load<Document>(document.Id);
                Console.WriteLine(loadedDoc.Contents);
            }
            Console.ReadLine();
        }
    
    }
}
