using System;

namespace GuildWars2.TradingPost.Exceptions
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException()
            : base("Login request failed to produce a session key")
        { }
    }
}
