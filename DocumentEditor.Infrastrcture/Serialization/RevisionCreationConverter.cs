using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DocumentEditor.Core.Models;
using Raven.Imports.Newtonsoft.Json.Converters;

namespace DocumentEditor.Infrastrcture.Serialization
{
    public class RevisionCreationConverter : CustomCreationConverter<IRevision>
    {
        public override IRevision Create(Type objectType)
        {
            var revision = Activator.CreateInstance(objectType, true);
            return (IRevision)revision;
        }

        public override object ReadJson(Raven.Imports.Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Raven.Imports.Newtonsoft.Json.JsonSerializer serializer)
        {
            if (existingValue != null)
                return base.ReadJson(reader, existingValue.GetType(), existingValue, serializer);
            else
                return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return base.CanConvert(objectType);
        }
    }
}
