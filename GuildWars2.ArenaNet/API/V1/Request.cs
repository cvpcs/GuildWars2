using System;
using System.Collections.Generic;

using RestSharp;

namespace GuildWars2.ArenaNet.API.V1
{
    public abstract partial class Request<T>
        where T : class, new()
    {
        public static int Timeout = 10000;
        public static readonly string URL = "https://api.guildwars2.com";
        public static readonly string Version = "v1";

        protected abstract string APIPath { get; }

        protected virtual Dictionary<string, string> APIParameters
        {
            get { return new Dictionary<string, string>(); }
        }

        protected virtual Method APIMethod
        {
            get { return Method.GET; }
        }
    }
}
