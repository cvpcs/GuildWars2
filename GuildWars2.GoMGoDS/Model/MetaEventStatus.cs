using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.GoMGoDS.Model
{
    public class MetaEventStatus
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("minCountdown")]
        public uint MinCountdown { get; set; }

        [JsonProperty("maxCountdown")]
        public uint MaxCountdown { get; set; }

        [JsonIgnore]
        public uint Countdown
        {
            set
            {
                MinCountdown = value;
                MaxCountdown = value;
            }
        }

        [JsonProperty("stageId")]
        public int StageId { get; set; }

        [JsonProperty("stageName")]
        public string StageName { get; set; }

        [JsonProperty("stageType")]
        public string StageType { get; set; }

        [JsonIgnore]
        public MetaEventStage.StageType StageTypeEnum
        {
            get
            {
                MetaEventStage.StageType type;
                if (Enum.TryParse<MetaEventStage.StageType>(StageType, true, out type))
                    return type;

                return MetaEventStage.StageType.Invalid;
            }
            set
            {
                StageType = Enum.GetName(typeof(MetaEventStage.StageType), value).ToLower();
            }
        }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }
}
