using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentEditor.Commands.DTOs;
using DocumentEditor.Web.Serialization;
using NUnit.Framework;
using Newtonsoft.Json;

namespace DocumentEditor.Web.Tests.Serialization
{
    public class DiffConverterTests
    {
        [Test]
        public void Ensure_We_Can_Create_A_Diff_Coverter()
        {
            var converter = new DiffConverter();
            var serializer = new JsonSerializer();
            serializer.Converters.Add(converter);
            var reader =
                new JsonTextReader(
                    new StringReader(
                        "{'DocumentId':'70a2c689-302c-4c4d-93a2-dcc379ace44a','RevisionId':'b35e5f5a-2033-4773-9bb9-557d0732a8f1','Patches':[{'diffs':[[0,'test test'],[1,' test']],'start1':0,'start2':0,'length1':9,'length2':14}]}"));
            var req =
                serializer.Deserialize<DocumentEditRequest>(reader);

            Assert.That(req.DocumentId, Is.EqualTo("70a2c689-302c-4c4d-93a2-dcc379ace44a"));
            Assert.That(req.Patches.Count(), Is.EqualTo(1));
            Assert.That(req.Patches[0].diffs.Count(), Is.EqualTo(2));
        }
    }
}
