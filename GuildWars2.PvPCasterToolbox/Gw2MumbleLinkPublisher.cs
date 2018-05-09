using System;
using System.Threading;
using System.Threading.Tasks;
using GW2NET.MumbleLink;
using Microsoft.Extensions.Logging;

namespace GuildWars2.PvPCasterToolbox
{
    public class Gw2MumbleLinkPublisher : AsyncDataPublisherBase<Avatar>
    {
        private long lastUiTick = -1;
        private MumbleLinkFile mumbleLinkFile = MumbleLinkFile.CreateOrOpen();

        public Gw2MumbleLinkPublisher(ILogger<Gw2MumbleLinkPublisher> logger)
            : base(logger)
        { }

        protected override Task<Avatar> GetDataAsync(CancellationToken token)
        {
            Avatar avatar = this.mumbleLinkFile.Read();
            if (avatar != null)
            {
                return Task.FromResult(avatar.UiTick > lastUiTick ? avatar : null);
            }
            else
            {
                throw new InvalidOperationException("MumbleLink data is not available");
            }
        }
    }
}
