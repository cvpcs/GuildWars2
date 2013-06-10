using System;
using System.Collections.Generic;

namespace GuildWars2.TradingPost.API
{
    public class SearchRequest : CountedRequest<SearchResponse>
    {
        public string Text { get; set; }
        public int RarityId { get; set; }
        public int TypeId { get; set; }
        public int SubtypeId { get; set; }
        public int LevelMin { get; set; }
        public int LevelMax { get; set; }
        public bool RemoveUnavailable { get; set; }

        protected override string APIPath
        {
            get { return "/ws/search.json"; }
        }

        protected override Dictionary<string, string> APIParameters
        {
            get
            {
                Dictionary<string, string> parameters = base.APIParameters;

                parameters["text"] = string.Empty;
                parameters["rarity"] = string.Empty;
                parameters["type"] = string.Empty;
                parameters["subtype"] = string.Empty;
                parameters["levelmin"] = string.Empty;
                parameters["levelmax"] = string.Empty;
                parameters["removeunavailable"] = (RemoveUnavailable ? "1" : "0");

                if (!string.IsNullOrWhiteSpace(Text))
                    parameters["text"] = Text;

                if (RarityId >= 0)
                    parameters["rarity"] = RarityId.ToString();

                if (TypeId >= 0)
                    parameters["type"] = TypeId.ToString();

                if (SubtypeId >= 0)
                    parameters["subtype"] = SubtypeId.ToString();

                if (LevelMin >= 0 && LevelMin <= 80)
                    parameters["levelmin"] = LevelMin.ToString();

                if (LevelMax >= 0 && LevelMax <= 80)
                    parameters["levelmax"] = LevelMax.ToString();

                return parameters;
            }
        }

        public SearchRequest(string txt = null, int rid = -1, int tid = -1, int stid = -1, int lmin = -1, int lmax = -1, bool ru = false)
        {
            Text = txt;
            RarityId = rid;
            TypeId = tid;
            SubtypeId = stid;
            LevelMin = lmin;
            LevelMax = lmax;
            RemoveUnavailable = ru;
        }
    }
}
