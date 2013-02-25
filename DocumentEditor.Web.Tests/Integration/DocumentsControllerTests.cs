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
using Newtonsoft.Json;

namespace DocumentEditor.Web.Tests.Integration
{
    public class DocumentsControllerTests : BaseIntegrationTestClass
    {
        [Test]
        public void Ensure_That_We_Can_Retrieve_Document_Data()
        {
            var document = PopulateDatabaseWithSingleDocument("Test");
            var client = new HttpClient(Server);
            var result = client.GetAsync(Url + "/api/documents/" + document.Id).Result;

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
            
            var data = new DocumentEditRequest
                {
                    Patches = Patches.Make("Test", "Test2").ToArray(),
                    DocumentId = document.Id,
                    RevisionId = document.CurrentRevision.Id
                };
            
            var request = client.PutAsJsonAsync(Url + "/api/documents/1", data);
            var result = request.Result;
            //sAssert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void Ensure_That_We_Can_Post_An_String_Edit_To_A_Document()
        {
            var document = PopulateDatabaseWithSingleDocument("Test");
            var client = new HttpClient(Server);
            var stringData = "{'DocumentId':'"+document.Id+"'," +
                             "'ParentRevisionId':'" + document.CurrentRevision.Id + "'," +
                             "'RevisionId' :' " + Guid.NewGuid() + "'," +
                             "'Patches':[{'diffs':[[0,'test test'],[1,' test']],'start1':0,'start2':0,'length1':9,'length2':14}]}";
           
            var content = new StringContent(stringData);
            var request = client.PutAsync(Url + "/api/documents/1",content);
            var result = request.Result;
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), result.Content.ReadAsStringAsync().Result);
        }
    }
}
