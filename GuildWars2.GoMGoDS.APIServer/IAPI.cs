using System;
using System.Data;

namespace GuildWars2.GoMGoDS.APIServer
{
    public interface IAPI
    {
        HttpJsonServer.RequestHandler RequestHandler { get; }

        void Start(IDbConnection dbConnection);
        void Stop();
    }
}
