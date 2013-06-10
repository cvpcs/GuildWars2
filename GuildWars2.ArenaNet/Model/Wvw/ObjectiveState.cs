using System;

namespace GuildWars2.ArenaNet.Model.Wvw
{
    public class ObjectiveState
    {
        public int Id { get; set; }

        public string Owner { get; set; }
        public ObjectiveOwnerType OwnerEnum
        {
            get
            {
                ObjectiveOwnerType type;
                if (Enum.TryParse<ObjectiveOwnerType>(Owner, true, out type))
                    return type;

                return ObjectiveOwnerType.Unknown;
            }
        }

        public Guid OwnerGuild { get; set; }
    }
}
