using System;

namespace GuildWars2.ArenaNet.Model
{
    public abstract class NamedModel<T>
    {
        public T Id { get; set; }

        public string Name { get; set; }
    }
}
