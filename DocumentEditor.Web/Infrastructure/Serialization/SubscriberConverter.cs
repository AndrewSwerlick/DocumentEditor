using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentEditor.Core.Models;
using Microsoft.AspNet.SignalR;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Converters;

namespace DocumentEditor.Web.Infrastructure.Serialization
{
    public class SubscriberConverter : CustomCreationConverter<Subscriber>
    {
        public override Subscriber Create(Type objectType)
        {
           return new Subscriber();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            var subscriber = (Subscriber) base.ReadJson(reader, objectType, existingValue, serializer);
            var context = GlobalHost.ConnectionManager.GetConnectionContext<DocumentConnection>();
            subscriber.SubscriberNotifiedOfUpdate +=
                (o, e) =>
                    {
                        context.Connection.Send(subscriber.Id, e);
                    };
            return subscriber;
        }
    }
}