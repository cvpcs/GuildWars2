using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.Model
{
    public class Task : MappedModel
    {
        public int TaskId { get; set; }

        public string Objective { get; set; }

        public int Level { get; set; }
    }
}
