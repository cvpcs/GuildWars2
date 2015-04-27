using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1.Wvw
{
    public class ObjectiveState
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }
        [JsonIgnore]
        public ObjectiveOwnerType OwnerEnum
        {
            get
            {
                ObjectiveOwnerType type;
                if (Enum.TryParse<ObjectiveOwnerType>(Owner, true, out type))
                    return type;

                return ObjectiveOwnerType.Invalid;
            }
        }

        [JsonProperty("owner_guild")]
        public Guid OwnerGuild { get; set; }
    }
}
