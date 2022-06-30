using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BooruBrowser.Lifecycle
{
    public class StateSerializationHelper
    {
        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full
        };

        public static string GetSerializedToString(object parameter)
        {
            return JsonConvert.SerializeObject(parameter, Formatting.Indented, _jsonSettings);
        }

        public static T GetDeserializedObject<T>(string parameter)
        {
            return JsonConvert.DeserializeObject<T>(parameter, _jsonSettings);
        }

        public static object GetDeserializedObject(string parameter)
        {
            return JsonConvert.DeserializeObject(parameter, _jsonSettings);
        }
    }
}
