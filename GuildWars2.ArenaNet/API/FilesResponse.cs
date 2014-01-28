using System;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class FilesResponse
    {
        [JsonProperty("map_complete")]
        public AssetFile MapComplete { get; set; }

        [JsonProperty("map_dungeon")]
        public AssetFile MapDungeon { get; set; }

        [JsonProperty("map_heart_empty")]
        public AssetFile MapHeartEmpty { get; set; }

        [JsonProperty("map_heart_full")]
        public AssetFile MapHeartFull { get; set; }

        [JsonProperty("map_node_harvesting")]
        public AssetFile MapNodeHarvesting { get; set; }

        [JsonProperty("map_node_logging")]
        public AssetFile MapNodeLogging { get; set; }

        [JsonProperty("map_node_mining")]
        public AssetFile MapNodeMining { get; set; }

        [JsonProperty("map_poi")]
        public AssetFile MapPoi { get; set; }

        [JsonProperty("map_special_event")]
        public AssetFile MapSpecialEvent { get; set; }

        [JsonProperty("map_story")]
        public AssetFile MapStory { get; set; }

        [JsonProperty("map_waypoint")]
        public AssetFile MapWaypoint { get; set; }

        [JsonProperty("map_waypoint_contested")]
        public AssetFile MapWaypointContested { get; set; }

        [JsonProperty("map_waypoint_hover")]
        public AssetFile MapWaypointHover { get; set; }
    }
}
