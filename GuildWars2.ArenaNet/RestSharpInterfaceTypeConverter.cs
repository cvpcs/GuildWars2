using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet
{
    public class RestSharpInterfaceTypeConverter<T1> : JsonConverter
        where T1 : new()
    {
        public override bool CanConvert(Type objectType)
        { return CanConvert(objectType, typeof(T1)); }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        { return ReadJson(reader, existingValue, serializer, typeof(T1)); }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        { serializer.Serialize(writer, value, value.GetType()); }

        protected bool CanConvert(Type interfaceType, params Type[] concreteTypes)
        {
            foreach (var concreteType in concreteTypes)
            {
                if (interfaceType.IsAssignableFrom(concreteType))
                    return true;
            }

            return false;
        }

        protected object ReadJson(JsonReader reader, object existingValue, JsonSerializer serializer, params Type[] concreteTypes)
        {
            object obj = null;

            foreach (var concreteType in concreteTypes)
            {
                obj = serializer.Deserialize(reader, concreteType);

                if (obj != null)
                    break;
            }

            return obj;
        }
    }

    public class RestSharpInterfaceTypeConverter<T1, T2> : RestSharpInterfaceTypeConverter<T1>
        where T1 : new()
        where T2 : new()
    {
        public override bool CanConvert(Type objectType)
        { return CanConvert(objectType, typeof(T1), typeof(T2)); }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        { return ReadJson(reader, existingValue, serializer, typeof(T1), typeof(T2)); }
    }
}
