using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

using RestSharp;

using GuildWars2.TradingPost.Exceptions;

namespace GuildWars2.TradingPost
{
    public class SessionManager
    {
        private static string m_CSV_SESSION_FILE = Path.Combine(Path.GetTempPath(), "GuildWars2.TradingPost.Sessions.csv");
        private static Regex m_SESSION_COOKIE_REGEX = new Regex("s=([0-9A-f]{8}-[0-9A-f]{4}-[0-9A-f]{4}-[0-9A-f]{4}-[0-9A-f]{12})", RegexOptions.Compiled);

        private static SessionManager m_SINGLETON = null;
        public static SessionManager GetInstance()
        {
            if (m_SINGLETON == null) { m_SINGLETON = new SessionManager(); }
            return m_SINGLETON;
        }

        private Session m_Session = null;
        public Session Session
        {
            get
            {
                if (m_Session == null)
                    m_Session = getSession();

                return m_Session;
            }
            set
            {
                m_Session = value;
            }
        }

        public Session GameSession
        {
            get
            {
                if (m_Session == null || !m_Session.IsGameSession)
                    m_Session = getSession(true);

                return m_Session;
            }
            set
            {
                saveSession(value);
                m_Session = value;
            }
        }

        public string Email { get; set; }
        public string Password { get; set; }

        private Session getSession(bool gameSession = false)
        {
            // get a list of sessions
            IList<Session> sessions = (File.Exists(m_CSV_SESSION_FILE) ? File.ReadAllLines(m_CSV_SESSION_FILE) : new string[] {})
                    .Select<string, Session>((line) =>
                    {
                        string[] record = line.Split(',');
                        if (record.Length == 3)
                        {
                            return new Session(record[0], bool.Parse(record[1]), DateTime.FromBinary(long.Parse(record[2])));
                        }
                        else
                        {
                            return new Session(string.Empty);
                        }
                    })
                    .Where(s => !string.IsNullOrWhiteSpace(s.Key))
                    .OrderByDescending(s => s.Created)
                    .ToList();

            // clear out our sessions
            File.Delete(m_CSV_SESSION_FILE);

            Session ret = null;
   
            foreach (Session session in sessions)
            {
                if (ret == null)
                {
                    if (!gameSession || gameSession == session.IsGameSession)
                    {
                        if (checkSessionAlive(session))
                            ret = session;
                        else
                            continue;
                    }
                }

                saveSession(session);
            }

            if (ret == null)
            {
                if (gameSession)
                    throw new GameSessionRequiredException();
                else
                    ret = getNewSession();
            }

            return ret;
        }

        private bool checkSessionAlive(Session session)
        {
            IRestResponse response = RequestUtil.NewInstance(Globals.TRADINGPOST_HOST, string.Empty)
                .SetCookie("s", session.Key)
                .Execute();

            if ((int)response.StatusCode < 400)
                return true; // good
            else if ((int)response.StatusCode == 503)
                return false; // expired
            else if ((int)response.StatusCode == 401)
                return false; // failed
            else
                throw new TradingPostOfflineException();
        }

        private Session getNewSession()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                throw new CredentialsNotFoundException();

            IRestResponse response = RequestUtil.NewInstance(Globals.AUTH_HOST, Globals.AUTH_PATH_LOGIN)
                .SetMethod(Method.POST)
                .SetParameter("email", Email)
                .SetParameter("password", Password)
                .SetFollowRedirects(false)
                .Execute();

            string key = null;

            foreach (Parameter header in response.Headers)
            {
                if (header.Name == "Set-Cookie")
                {
                    Match m = m_SESSION_COOKIE_REGEX.Match((string)header.Value);
                    if (m.Success)
                    {
                        key = m.Groups[1].Value;
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(key))
            {
                Session session = new Session(key);

                saveSession(session);

                return session;
            }
            else
                throw new LoginFailedException();
        }

        private void saveSession(Session session)
        {
            TextWriter tw = File.AppendText(m_CSV_SESSION_FILE);
            tw.WriteLine(session.Key + "," + session.IsGameSession.ToString() + "," +  session.Created.ToBinary());
            tw.Close();
        }
    }
}
