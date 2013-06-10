using System;
using System.Collections.Generic;

namespace GuildWars2.TradingPost.API
{
    public class MeRequest : CountedRequest<MeResponse>
    {
        public TimeType Time { get; set; }
        public ListingType Type { get; set; }
        public string CharId { get; set; }

        protected override string APIPath
        {
            get { return "/ws/me.json"; }
        }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                switch (Time)
                {
                    case TimeType.NOW:
                        parameters["time"] = "now";
                        break;
                    case TimeType.PAST:
                        parameters["time"] = "past";
                        break;
                    default:
                        break;
                }

                switch (Type)
                {
                    case ListingType.SELL:
                        parameters["type"] = "sell";
                        break;
                    case ListingType.BUY:
                        parameters["type"] = "buy";
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrWhiteSpace(CharId))
                    parameters["charid"] = CharId;

                return parameters;
            }
        }

        protected override bool RequiresGameSession
        {
            get { return true; }
        }

        public MeRequest(TimeType time = TimeType.NOW, ListingType type = ListingType.SELL)
        {
            Time = time;
            Type = type;
        }

        public enum ListingType { BUY, SELL }
        public enum TimeType { NOW, PAST }
    }
}
