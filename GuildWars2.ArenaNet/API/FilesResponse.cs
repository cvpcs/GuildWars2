using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class FilesResponse
    {
        public AssetFile MapComplete { get; set; }

        public AssetFile MapDungeon { get; set; }

        public AssetFile MapHeartEmpty { get; set; }

        public AssetFile MapHeartFull { get; set; }

        public AssetFile MapNodeHarvesting { get; set; }

        public AssetFile MapNodeLogging { get; set; }

        public AssetFile MapNodeMining { get; set; }

        public AssetFile MapPoi { get; set; }

        public AssetFile MapSpecialEvent { get; set; }

        public AssetFile MapStory { get; set; }

        public AssetFile MapWaypoint { get; set; }

        public AssetFile MapWaypointContested { get; set; }

        public AssetFile MapWaypointHover { get; set; }
    }
}
