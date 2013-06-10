using System;
using System.Collections.Generic;

namespace GuildWars2.TradingPost.Model
{
    public class Item
    {
        public int BuyCount { get; set; }

        public int BuyPrice { get; set; }

        public int Count { get; set; }

        public int DataId { get; set; }

        public string Description { get; set; }

        public string Img { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public List<PasswordEntry> Passwords { get; set; }

        public int Price { get; set; }

        public int Rarity { get; set; }

        public string RarityWord { get; set; }

        public int SellCount { get; set; }

        public int SellPrice { get; set; }

        public int TypeId { get; set; }

        public int Vendor { get; set; }
    }
}
