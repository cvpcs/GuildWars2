using System;
using System.Collections.Generic;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet.API.V1
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
