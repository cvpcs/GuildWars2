using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public abstract class NamedModel<T>
    {
        [JsonProperty("id")]
        public T Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
