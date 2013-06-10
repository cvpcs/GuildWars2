using System;

namespace GuildWars2.ArenaNet.Model
{
    public class Guild
    {
        public Guid GuildId { get; set; }

        public string GuildName { get; set; }

        public string Tag { get; set; }

        public GuildEmblem Emblem { get; set; }
    }
}
