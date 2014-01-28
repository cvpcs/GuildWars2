using System;
using System.IO;
using System.Globalization;
using System.Text;

using Newtonsoft.Json;

using RestSharp;
using RestSharp.Deserializers;

namespace GuildWars2.ArenaNet
{
    public class RestSharpNewtonSoftJsonDeserializer : IDeserializer
    {
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public CultureInfo Culture { get; set; }

        public RestSharpNewtonSoftJsonDeserializer()
        {
            Culture = CultureInfo.InvariantCulture;
        }

        public T Deserialize<T>(IRestResponse response)
        {
            T target;
            
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content)))
            using (StreamReader sr = new StreamReader(ms))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer s = new JsonSerializer();
                target = s.Deserialize<T>(jr);
            }

            return target;
        }
    }
}
