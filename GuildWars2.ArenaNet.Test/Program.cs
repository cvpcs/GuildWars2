using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;
using GuildWars2.ArenaNet.EventTimer;

namespace GuildWars2.ArenaNet.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {/*
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<MetaEventStatus>));
            FileInfo jsonFile = new FileInfo("event_timer.json");

            IList<MetaEventStatus> statusList = null;
            if (jsonFile.Exists)
            {
                FileStream stream = null;
                try
                {
                    stream = jsonFile.Open(FileMode.Open, FileAccess.Read);
                    statusList = (List<MetaEventStatus>)serializer.ReadObject(stream);
                }
                catch (Exception)
                { }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }

            if (statusList == null)
            {
                statusList = new List<MetaEventStatus>();
                foreach (MetaEvent meta in MetaEventDefinitions.MetaEvents)
                {
                    statusList.Add(new MetaEventStatus()
                        {
                            Id = meta.Id,
                            Name = meta.Name,
                            MinCountdown = meta.MinSpawn,
                            MaxCountdown = meta.MaxSpawn,
                            StageId = -1,
                            StageTypeEnum = MetaEventStage.StageType.Reset,
                            StageName = null,
                            Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds
                        });
                }
            }

            IDictionary<string, int> statusListMap = new Dictionary<string, int>();
            for (int i = 0; i < statusList.Count; i++)
            {
                MetaEventStatus status = statusList[i];
                statusListMap[status.Id] = i;
            }

            while (true)
            {
                // get current time
                DateTime startTime = DateTime.Now;

                // get data
                EventsResponse response = new EventsRequest(1007).Execute();

                IList<EventState> metaEvents = response.Events.Where(es => MetaEventDefinitions.EventList.Contains(es.EventId)).ToList();

                // discover meta-event states
                foreach (MetaEvent meta in MetaEventDefinitions.MetaEvents)
                {
                    MetaEvent.MetaEventState state = meta.GetState(metaEvents);

                    MetaEventStatus status = new MetaEventStatus()
                    {
                        Id = meta.Id,
                        Name = meta.Name,
                        MinCountdown = meta.MinSpawn,
                        MaxCountdown = meta.MaxSpawn,
                        StageId = state.StageId,
                        StageType = null,
                        StageName = null,
                        Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds
                    };

                    if (state.StageId >= 0)
                    {
                        MetaEventStage stage = meta.Stages[state.StageId];

                        if (stage.Countdown > 0 && stage.Countdown != uint.MaxValue)
                        {
                            status.Countdown = stage.Countdown;
                        }

                        status.StageName = stage.Name;
                        status.StageType = Enum.GetName(typeof(MetaEventStage.StageType), stage.Type).ToLower();
                    }

                    MetaEventStatus oldStatus = statusList[statusListMap[meta.Id]];

                    if (oldStatus.StageId != status.StageId)
                    {
                        statusList[statusListMap[meta.Id]] = status;

                        FileStream stream = null;
                        try
                        {
                            stream = jsonFile.Open(FileMode.Create, FileAccess.Write, FileShare.None);
                            serializer.WriteObject(stream, statusList);
                        }
                        catch (Exception)
                        { }
                        finally
                        {
                            if (stream != null)
                                stream.Close();
                        }
                    }
                }

                SpinWait.SpinUntil(() =>
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    return (ts.TotalSeconds > 30);
                });
            }*/
        }
    }
}
