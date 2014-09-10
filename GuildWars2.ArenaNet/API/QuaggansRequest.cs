using System;

namespace GuildWars2.ArenaNet.API
{
    public class QuaggansRequest : RequestV2<string, QuaggansResponse>
    {
        protected override string APIPath
        {
            get { return "/" + Version + "/quaggans"; }
        }

        public QuaggansRequest(params string[] quaggan_ids)
            : base(quaggan_ids)
        { }
    }
}
