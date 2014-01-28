using System;

using Newtonsoft.Json;

namespace GuildWars2.GoMGoDS.Model
{
    public class NodeLocation
    {
        [JsonProperty("x")]
        public int X;

        [JsonProperty("y")]
        public int Y;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("type")]
        public string Type;
    }
}
