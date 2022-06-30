using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooruBrowser.Extensions
{
    public class IgnoreAttributesContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);

            foreach (JsonProperty prop in list)
            {
                prop.PropertyName = prop.UnderlyingName;
            }

            return list;
        }
    }
}
