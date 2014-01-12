using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;

using Mono.Data.Sqlite;

namespace GuildWars2.GoMGoDS.APIServer.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  {0} api_class_name [key=value key=value ...]", Assembly.GetExecutingAssembly().GetName().Name);
            }
            else
            {
                if (args[0].Equals("?"))
                {
                    Console.WriteLine("Available APIs to test:");

                    foreach (string type in GetAPITypes())
                        Console.WriteLine("  {0}", type);
                }
                else
                {
                    IDbConnection db = new SqliteConnection(string.Format("Data Source={0}", ConfigurationManager.AppSettings["sqlite_db"]));
                    db.Open();

                    IAPI api = GetAPI(args[0]);
                    if (api == null)
                    {
                        Console.WriteLine("{0} not found", args[1]);
                    }
                    else
                    {
                        IDictionary<string, string> get = new Dictionary<string, string>();
                        for (int i = 1; i < args.Length; i++)  
                        {
                            int idx = args[i].IndexOf('=');
                            if (idx < 0 || idx + 1 >= args[i].Length)
                            {
                                get.Add(args[i], true.ToString());
                            }
                            else
                            {
                                get.Add(args[i].Substring(0, idx), args[i].Substring(idx + 1));
                            }
                        }

                        api.Init(db);
                        File.WriteAllText(ConfigurationManager.AppSettings["json_file"], api.RequestHandler(get));
                    }

                    db.Close();
                }
            }

            Console.WriteLine();
        }

        private static IList<string> GetAPITypes()
        {
            IList<string> apis = new List<string>();

            Assembly assembly = Assembly.GetAssembly(typeof(IAPI));
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                    continue;

                Type[] itypes = type.GetInterfaces();

                foreach (Type itype in itypes)
                {
                    if (itype == typeof(IAPI))
                    {
                        apis.Add(type.Name);
                        break;
                    }
                }
            }

            return apis;
        }

        private static IAPI GetAPI(string name)
        {
            IAPI api = null;

            Assembly assembly = Assembly.GetAssembly(typeof(IAPI));
            Type type = assembly.GetType(string.Format("{0}.{1}", assembly.GetName().Name, name));
            if (type != null)
            {
                Type[] itypes = type.GetInterfaces();

                foreach (Type itype in itypes)
                {
                    if (itype == typeof(IAPI))
                    {
                        api = (IAPI)assembly.CreateInstance(type.FullName, true);
                        break;
                    }
                }
            }

            return api;
        }
    }
}
