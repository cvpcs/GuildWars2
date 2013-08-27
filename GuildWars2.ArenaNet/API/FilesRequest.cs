using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.API
{
    public class FilesRequest : Request<FilesResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/files.json"; }
        }

        public FilesRequest()
            : base()
        { }
    }
}
