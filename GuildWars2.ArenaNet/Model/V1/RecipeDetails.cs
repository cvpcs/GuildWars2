using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace GuildWars2.ArenaNet.Model.V1
{
    public class RecipeDetails
    {
        [JsonProperty("recipe_id")]
        public int RecipeId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonIgnore]
        public RecipeType TypeEnum
        {
            get
            {
                RecipeType type;
                if (Enum.TryParse<RecipeType>(Type, true, out type))
                    return type;

                return RecipeType.Invalid;
            }
        }

        [JsonProperty("output_item_id")]
        public int OutputItemId { get; set; }

        [JsonProperty("output_item_count")]
        public int OutputItemCount { get; set; }

        [JsonProperty("min_rating")]
        public int MinRating { get; set; }

        [JsonProperty("time_to_craft_ms")]
        public int TimeToCraftMs { get; set; }

        [JsonProperty("vendor_value")]
        public int VendorValue { get; set; }

        [JsonProperty("disciplines")]
        public List<string> Disciplines { get; set; }
        [JsonIgnore]
        public DisciplineType DisciplinesEnum
        {
            get
            {
                DisciplineType typeAll = DisciplineType.None;

                foreach (string item in Disciplines)
                {
                    DisciplineType type;
                    if (Enum.TryParse<DisciplineType>(item, true, out type))
                        typeAll |= type;
                    else
                        typeAll |= DisciplineType.Invalid;
                }

                return typeAll;
            }
        }

        [JsonProperty("flags")]
        public List<string> Flags { get; set; }
        [JsonIgnore]
        public RecipeFlagType FlagsEnum
        {
            get
            {
                RecipeFlagType typeAll = RecipeFlagType.None;

                foreach(string item in Flags)
                {
                    RecipeFlagType type;
                    if (Enum.TryParse<RecipeFlagType>(item, true, out type))
                        typeAll |= type;
                    else
                        typeAll |= RecipeFlagType.Invalid;
                }

                return typeAll;
            }
        }

        [JsonProperty("ingredients")]
        public List<Ingredient> Ingredients { get; set; }
    }
}
