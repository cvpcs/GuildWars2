using System;
using System.Collections.Generic;

using RestSharp;

namespace GuildWars2.ArenaNet.API.V2
{
    public abstract partial class Request<TRID, TRDR> : V1.Request<TRDR>
        where TRDR : class, new()
    {
        public new static readonly string Version = "v2";

        private List<TRID> m_Ids;

        public TRID Id 
        {
            get
            {
                if (m_Ids.Count > 0)
                    return m_Ids[0];
                else
                    return default(TRID);
            }
            set
            {
                m_Ids.Clear();
                m_Ids.Add(value);
            }
        }

        public List<TRID> Ids
        {
            get { return m_Ids; }
            set { m_Ids = value; }
        }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                if (m_Ids.Count > 0)
                    parameters["ids"] = string.Join(",", m_Ids);

                return parameters;
            }
        }

        public Request(params TRID[] ids)
        {
            m_Ids = new List<TRID>(ids);
        }
    }
}
