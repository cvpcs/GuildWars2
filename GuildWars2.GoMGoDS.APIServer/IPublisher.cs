using System;

namespace GuildWars2.GoMGoDS.APIServer
{
    public interface IPublisher
    {
        void Start();
        void Stop();
    }

    public interface IPublisher<T> : IPublisher
    {
        void RegisterSubscriber(ISubscriber<T> subscriber);
    }
}
