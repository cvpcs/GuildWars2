using System;
using System.Collections.Generic;
using System.Linq;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.GoMGoDS.Model
{
    public class ContestedLocation
    {
        public string Id { get { return Abbreviation.ToLower(); } }
        public string Name { get; private set; }
        public string Abbreviation { get; private set; }

        public HashSet<Guid> CaptureEvents { get; private set; }
        public HashSet<Guid> DefendEvents { get; private set; }

        public HashSet<Guid> Events
        { get { return new HashSet<Guid>(CaptureEvents.Union(DefendEvents).Distinct()); } }

        public ContestedLocation(string name, string abbreviation)
        {
            Name = name;
            Abbreviation = abbreviation;

            CaptureEvents = new HashSet<Guid>();
            DefendEvents = new HashSet<Guid>();
        }

        public ContestedLocation AddCaptureEvent(Guid eid)
        {
            CaptureEvents.Add(eid);
            return this;
        }

        public ContestedLocation AddDefendEvent(Guid eid)
        {
            DefendEvents.Add(eid);
            return this;
        }
    }
}
