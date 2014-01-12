using System;

using GuildWars2.ArenaNet.API;

namespace GuildWars2.GoMGoDS.APIServer
{
    public class BuildIdPublisher : PublisherBase<BuildResponse>
    {
        private static TimeSpan p_PollRate = new TimeSpan(0, 0, 30);

        public BuildIdPublisher()
            : base(p_PollRate)
        {
            m_Data = new BuildResponse();
            m_Data.BuildId = int.MinValue;
        }

        protected override bool UpdateData()
        {
            BuildResponse build = new BuildRequest().Execute();

            if (build != null && build.BuildId != m_Data.BuildId)
            {
                m_Data = build;
                return true;
            }

            return false;
        }
    }
}
