using System;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class AssetFile
    {
        [JsonProperty("file_id")]
        public int FileId { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}
