using System;

namespace GuildWars2.GoMGoDS.APIServer
{
    public interface ISubscriber<T>
    {
        void Process(T data);
    }
}
