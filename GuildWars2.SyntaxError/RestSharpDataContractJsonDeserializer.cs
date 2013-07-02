using System;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization.Json;
using System.Text;

using RestSharp;
using RestSharp.Deserializers;

namespace GuildWars2.SyntaxError
{
    public class RestSharpDataContractJsonDeserializer : IDeserializer
    {
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public CultureInfo Culture { get; set; }

        public RestSharpDataContractJsonDeserializer()
        {
            Culture = CultureInfo.InvariantCulture;
        }

        public T Deserialize<T>(IRestResponse response)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            T target = (T)ser.ReadObject(ms);
            ms.Close();

            return target;
        }
    }
}
