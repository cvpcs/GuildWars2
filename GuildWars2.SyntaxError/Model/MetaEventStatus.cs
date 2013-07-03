using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuildWars2.SyntaxError.Model
{
    [DataContract]
    public class MetaEventStatus
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "minCountdown")]
        public uint MinCountdown { get; set; }

        [DataMember(Name = "maxCountdown")]
        public uint MaxCountdown { get; set; }

        public uint Countdown
        {
            set
            {
                MinCountdown = value;
                MaxCountdown = value;
            }
        }

        [DataMember(Name = "stageId")]
        public int StageId { get; set; }

        [DataMember(Name = "stageName")]
        public string StageName { get; set; }

        [DataMember(Name = "stageType")]
        public string StageType { get; set; }

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

        [DataMember(Name = "timestamp")]
        public long Timestamp { get; set; }
    }
}
