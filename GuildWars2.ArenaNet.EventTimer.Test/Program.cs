using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;
using GuildWars2.ArenaNet.EventTimer;
using GuildWars2.GoMGoDS.Model;

namespace GuildWars2.ArenaNet.EventTimer.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  GuildWars2.ArenaNet.EventTimer.Test.exe [meta_id] [file]");
                Console.WriteLine();
                return;
            }

            FileInfo csv = new FileInfo(args[1]);
            MetaEvent meta = MetaEventDefinitions.MetaEvents.Where(m => m.Id == args[0]).Single();

            if (meta == null)
            {
                Console.WriteLine("Could not find meta-event {0}", args[0]);
                return;
            }

            IList<Guid> events = meta.Stages.SelectMany(s => s.EventStates).Select(s => s.Event).Distinct().ToList();

            EventNamesResponse namesResponse = new EventNamesRequest().Execute();
            if (namesResponse == null)
            {
                Console.WriteLine("Could not retrieve event names");
                return;
            }

            IDictionary<Guid, string> names = namesResponse.ToDictionary(n => n.Id, n => n.Name);

            DataTable table = new DataTable();
            table.Columns.Add("Timestamp");
            foreach (Guid ev in events)
            {
                table.Columns.Add(ev.ToString());
            }
            table.Columns.Add("MetaState");

            Console.WriteLine("Beginning status tracking. Please press any key to exit.");

            while (!Console.KeyAvailable)
            {
                EventsResponse response = new EventsRequest(1007).Execute();
                if (response != null)
                {
                    HashSet<EventState> states = new HashSet<EventState>(response.Events.Where(e => events.Contains(e.EventId)));
                    int stageId = meta.GetStageId(states);

                    DateTime timestamp = DateTime.Now;
                    DataRow row = table.NewRow();

                    foreach (DataColumn column in table.Columns)
                    {
                        if (column.ColumnName == "Timestamp")
                            row[column] = timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                        else if (column.ColumnName == "MetaState")
                            row[column] = (stageId >= 0 ? meta.Stages.ElementAt(stageId).Name : "Inactive");
                        else
                        {
                            Guid eventId = new Guid(column.ColumnName);
                            row[column] = states.Where(e => e.EventId == eventId).Select(e => e.State).Single();
                        }
                    }

                    DataRow lastRow = (table.Rows.Count == 0 ? table.NewRow() : table.Rows[table.Rows.Count - 1]);
                    bool stateChanged = false;
                    foreach (DataColumn column in table.Columns)
                    {
                        if (column.ColumnName == "Timestamp")
                            continue;

                        if (!lastRow[column].Equals(row[column]))
                        {
                            stateChanged = true;
                            break;
                        }
                    }

                    if (stateChanged)
                    {
                        Console.WriteLine("Recording state change...");
                        table.Rows.Add(row);
                    }
                }

                Thread.Sleep(5000);
            }

            try
            {
                using (FileStream stream = csv.Open(FileMode.Create))
                {
                    StreamWriter writer = new StreamWriter(stream);

                    foreach (DataColumn column in table.Columns)
                    {
                        IList<string> values = new List<string>();

                        if (column.ColumnName != "Timestamp" &&
                            column.ColumnName != "MetaState")
                        {
                            Guid ev = new Guid(column.ColumnName);
                            values.Add(names[ev]);
                        }
                        else
                        {
                            values.Add(string.Empty);
                        }

                        foreach (DataRow row in table.Rows)
                        {
                            values.Add(row[column].ToString());
                        }

                        string data = string.Join(",", values.Select(s => (s.Contains(",") ? string.Format("\"{0}\"", s) : s)));

                        writer.WriteLine(data);
                    }

                    writer.Close();
                }
            }
            catch
            { }
        }
    }
}
