using System;
using System.Collections.Generic;

namespace GuildWars2.ArenaNet.API.V1
{
    public class GuildDetailsRequest : Request<GuildDetailsResponse>
    {
        public Guid? GuildId { get; set; }
        public string GuildName { get; set; }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                if (GuildId.HasValue)
                    parameters["guild_id"] = GuildId.Value.ToString();
                else if (!string.IsNullOrEmpty(GuildName))
                    parameters["guild_name"] = GuildName;

                return parameters;
            }
        }

        protected override string APIPath
        {
            get { return "/" + Version + "/guild_details.json"; }
        }

        public GuildDetailsRequest(Guid guild_id)
        {
            GuildId = guild_id;
        }

        public GuildDetailsRequest(string guild_name)
        {
            GuildName = guild_name;
        }
    }
}
