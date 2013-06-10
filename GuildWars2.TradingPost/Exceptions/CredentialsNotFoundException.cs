using System;

namespace GuildWars2.TradingPost.Exceptions
{
    public class CredentialsNotFoundException : Exception
    {
        public CredentialsNotFoundException()
            : base("Email and/or password not provided")
        { }
    }
}
