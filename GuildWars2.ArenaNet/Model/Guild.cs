using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class Guild
    {
        [JsonProperty("guild_id")]
        public Guid GuildId { get; set; }

        [JsonProperty("guild_name")]
        public string GuildName { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("emblem")]
        public GuildEmblem Emblem { get; set; }
    }
}
