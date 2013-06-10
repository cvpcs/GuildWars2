using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildWars2.TradingPost
{
    public class Session
    {
        public string Key { get; set; }
        public bool IsGameSession { get; set; }
        public DateTime Created { get; set; }

        public Session(string key, bool gameSession, DateTime created)
        {
            Key = key;
            IsGameSession = gameSession;
            Created = created;
        }

        public Session(string key, bool gameSession)
            : this(key, gameSession, DateTime.Now)
        { }

        public Session(string key)
            : this(key, false, DateTime.Now)
        { }
    }
}
