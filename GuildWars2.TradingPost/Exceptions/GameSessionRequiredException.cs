using System;

namespace GuildWars2.TradingPost.Exceptions
{
    public class GameSessionRequiredException : Exception
    {
        public GameSessionRequiredException()
            : base("A game session token is required to use this API")
        { }
    }
}
