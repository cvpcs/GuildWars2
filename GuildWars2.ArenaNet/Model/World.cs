using System;

namespace GuildWars2.ArenaNet.Model
{
    public class World : NamedModel<int>
    {
        public RegionType Region
        {
            get
            {
                if (Id >= 1000 && Id < 2000)
                    return RegionType.NA;
                else if (Id >= 2000 && Id < 3000)
                    return RegionType.EU;
                else
                    return RegionType.Unknown;
            }
        }
    }
}
