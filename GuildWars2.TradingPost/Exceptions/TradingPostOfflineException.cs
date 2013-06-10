using System;

namespace GuildWars2.TradingPost.Exceptions
{
    public class TradingPostOfflineException : Exception
    {
        public TradingPostOfflineException()
            : base("The Trading Post appears to be offline")
        { }
    }
}
