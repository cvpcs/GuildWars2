using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model
{
    public class ItemDetailsV2 : ItemDetails
    {
        [JsonProperty("id")]
        public new int ItemId { get; set; }

        [JsonProperty("icon")]
        public Uri IconUri { get; set; }

        [JsonIgnore]
        private new int IconFileId { get; set; }

        [JsonIgnore]
        private new string IconFileSignature { get; set; }

        [JsonIgnore]
        private new AssetFile IconFile { get; set; }

        // armor?
        // weapon?
        // other stats?
    }
}
