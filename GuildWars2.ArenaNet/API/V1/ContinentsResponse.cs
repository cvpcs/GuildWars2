﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
{
    public class ContinentsResponse
    {
        [JsonProperty("continents")]
        public Dictionary<int, Continent> Continents { get; set; }
    }
}
