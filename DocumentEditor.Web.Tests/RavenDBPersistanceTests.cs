using DocumentEditor.Core.Models;
using DocumentEditor.Core.Tests.Util;
using DocumentEditor.Web.Serialization;
using NUnit.Framework;
using Raven.Client.Embedded;

namespace DocumentEditor.Web.Tests
{
    public class RavenDBPersistanceTests
    {
        private EmbeddableDocumentStore DocumentStore { get; set; }

        [SetUp]
        public void Setup()
        {
            DocumentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true,
                    Conventions =
                        {
                            CustomizeJsonSerializer = serializer =>
                                                      serializer.ContractResolver =
                                                      new FluentContractResolver().MarkAsReference<IRevision>()
                        },

                };
            DocumentStore.Initialize();
        }
     
        [Test]
        public void
            Ensure_That_When_We_Save_And_Then_Reload_a_Document_That_The_NextRevision_Applied_Property_On_Its_Revisios_Is_Populated
            ()
        {
            Document document;
            using (var session = DocumentStore.OpenSession())
            {
                document = new Document("Test");
                var revision = new BasicRevision(document.CurrentRevision, Patches.Make(document.Contents, "Test2"));
                document.Edit(revision);
                session.Store(document);

                session.SaveChanges();
            }

            using (var session = DocumentStore.OpenSession())
            {
                var loadedDoc = session.Load<Document>(document.Id);
                Assert.That(loadedDoc.CurrentRevision.PreviousRevisionAppliedTo.NextRevisionApplied, Is.EqualTo(loadedDoc.CurrentRevision));
            }
        }
    }
}
