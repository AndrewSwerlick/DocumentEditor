using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentEditor.Core.Models;
using DocumentEditor.Web.Models;
using NUnit.Framework;

namespace DocumentEditor.Web.Tests.Integration
{
    public class DocumentsControllerTests : BaseIntegrationTestClass
    {
        [Test]
        public void Ensure_That_We_Can_Retrieve_Document_Data()
        {
            var document = new Document("Test");
            using (Session)
            {
                Session.Store(document);
                Session.SaveChanges();
            }
            var client = new HttpClient(Server);
            var result = client.GetAsync(Url + "/api/documents/1").Result;

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Content.ReadAsAsync<DocumentData>().Result.Contents, Is.EqualTo("Test") );
        }
    
    }
}
