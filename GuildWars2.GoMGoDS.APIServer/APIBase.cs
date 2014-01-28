using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

using log4net;
using Newtonsoft.Json;

namespace GuildWars2.GoMGoDS.APIServer
{
    public abstract class APIBase<T> : IAPI
    {
        private static ILog LOGGER = null;

        private static readonly bool INDENT_JSON = bool.TrueString.Equals(ConfigurationManager.AppSettings["indent_json"], StringComparison.CurrentCultureIgnoreCase);

        public abstract string RequestPath { get; }
        public HttpJsonServer.RequestHandler RequestHandler { get { return GetJson; } }

        public APIBase()
        {
            if (LOGGER == null)
                LOGGER = LogManager.GetLogger(this.GetType());
        }

        public abstract void Init(IDbConnection dbConn);

        protected abstract T GetData(IDictionary<string, string> _get);

        protected virtual string GetJson(IDictionary<string, string> _get)
        {
            string json = string.Empty;
            T data = GetData(_get);

            try
            {
                json = JsonConvert.SerializeObject(data, (INDENT_JSON ? Formatting.Indented : Formatting.None));
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to serialize JSON", e);
            }

            return json;
        }
    }
}
