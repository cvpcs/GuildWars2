using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuildWars2.GoMGoDS.APIServer
{
    public abstract class CachedAPI<T> : APIBase<T>
    {
        private TimeSpan m_CacheTimeout;
        private DateTime m_CacheTimestamp = DateTime.MinValue;

        public CachedAPI(TimeSpan cacheTimeout)
        {
            m_CacheTimeout = cacheTimeout;
        }

        protected virtual DateTime GetCacheTimestamp(IDictionary<string, string> _get)
        {
            return m_CacheTimestamp;
        }

        protected abstract void RefreshData(IDictionary<string, string> _get);

        protected override string GetJson(IDictionary<string, string> _get)
        {
            DateTime timestamp = GetCacheTimestamp(_get);
            if (DateTime.UtcNow - timestamp > m_CacheTimeout)
            {
                m_CacheTimestamp = DateTime.UtcNow;
                RefreshData(_get);
            }

            return base.GetJson(_get);
        }
    }
}
