using System;
using System.Collections.Generic;

using RestSharp;

namespace GuildWars2.SyntaxError.API
{
    public abstract partial class Request<T>
        where T : class, new()
    {
        public static int Timeout = 10000;

        private static string URL = "http://synt4x3rr0r.net";

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
