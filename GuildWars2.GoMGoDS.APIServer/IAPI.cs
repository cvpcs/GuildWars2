using System;
using System.Data;

namespace GuildWars2.GoMGoDS.APIServer
{
    public interface IAPI
    {
        string RequestPath { get; }
        HttpJsonServer.RequestHandler RequestHandler { get; }

        void Init(IDbConnection dbConn);
    }
}
