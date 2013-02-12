using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Serialization;

namespace DocumentEditor.Infrastrcture.Serialization
{
    public class FluentContractResolver : DefaultContractResolver
    {
        private readonly IDictionary<Type,string> _ignoredProperties = new Dictionary<Type, string>();
        private readonly IList<Type> _referenceTypes = new List<Type>(); 

        public FluentContractResolver IgnoreProperty<TSource>(
            Expression<Func<TSource, object>> propertyLambda)
        {
            var name = PropertyName.For(propertyLambda);
            _ignoredProperties.Add(typeof(TSource), name);
            return this;
        }
        public FluentContractResolver MarkAsReference<TSource>()
        {
           _referenceTypes.Add(typeof(TSource));
            return this;
        }


        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {            
            var properties = base.CreateProperties(type, memberSerialization).ToList();
            var types = _ignoredProperties.Keys.Where(k => k.IsAssignableFrom(type));
            var propsToIgnore = types.Select(t => _ignoredProperties[t]);

            properties.RemoveAll(p => propsToIgnore.Contains(p.PropertyName));            
            return properties;
        }
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {            
            var prop = base.CreateProperty(member, memberSerialization);
            
            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            if (_referenceTypes.Any(t=> t.IsAssignableFrom(objectType)))
            {
                contract.IsReference = true;
            }
            return contract;
        }
    }
}
