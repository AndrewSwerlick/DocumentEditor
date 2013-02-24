using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LayoutEditor.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DocumentEditor.Web.Serialization
{
    public class DiffConverter : CustomCreationConverter<Diff>
    {
        private static IDictionary<int, Operation> _opMap = new Dictionary<int, Operation>
            {
                {-1,Operation.DELETE},
                {0,Operation.EQUAL},
                {1,Operation.INSERT}
            }; 

        public override Diff Create(Type objectType)
        {
            return new Diff(Operation.EQUAL,"");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var diffArray = serializer.Deserialize<string[]>(reader);
            var op = _opMap[int.Parse(diffArray[0])];
            var diff = new Diff(op, diffArray[1]);
            return diff;
        }
    }
}