using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Json;

using log4net;

namespace GuildWars2.GoMGoDS.APIServer
{
    public abstract class APIBase<T> : IAPI
    {
        private static ILog LOGGER = null;

        private static DataContractJsonSerializer p_Serializer = new DataContractJsonSerializer(typeof(T));

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
                MemoryStream stream = new MemoryStream();
                p_Serializer.WriteObject(stream, data);
                stream.Flush();
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                json = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception thrown when attempting to serialize JSON", e);
            }

            return json;
        }
    }
}
