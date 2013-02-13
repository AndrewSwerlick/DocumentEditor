using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentEditor.Commands.DTOs;
using DocumentEditor.Core.Models;
using DocumentEditor.Core.Tests.Util;
using DocumentEditor.Web.Models;
using NUnit.Framework;

namespace DocumentEditor.Web.Tests.Integration
{
    public class DocumentsControllerTests : BaseIntegrationTestClass
    {
        [Test]
        public void Ensure_That_We_Can_Retrieve_Document_Data()
        {
            PopulateDatabaseWithSingleDocument("Test");
            var client = new HttpClient(Server);
            var result = client.GetAsync(Url + "/api/documents/1").Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Content.ReadAsAsync<DocumentData>().Result.Contents, Is.EqualTo("Test") );
        }
    
        [Test]
        public void Ensure_That_We_Can_Create_A_Document()
        {
            var client = new HttpClient(Server);
            var result = client.PostAsJsonAsync(Url + "/api/documents/", new DocumentCreationRequest{Name = "TestDoc"}).Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            using (var session = DocumentStore.OpenSession())
            {
                Assert.That(session.Query<Document>().Count(d => d.Name == "TestDoc"), Is.EqualTo(1));
            }
        }

        [Test]
        public void Ensure_That_We_Can_Post_An_Edit_To_A_Document()
        {
            var document = PopulateDatabaseWithSingleDocument("Test");
            var client = new HttpClient(Server);
            
            var request = new DocumentEditRequest
                {
                    Patches = Patches.Make("Test", "Test2"),
                    DocumentId = document.Id,
                    RevisionId = document.CurrentRevision.Id
                };
            var result = client.PutAsJsonAsync(Url + "/api/documents/1", request).Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}
