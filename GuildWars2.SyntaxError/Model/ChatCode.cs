using System;
using System.Collections.Generic;

namespace GuildWars2.SyntaxError.Model
{
    public class ChatCode
    {
        public ChatCodeType Header
        {
            get
            {
                if (Data != null && Data.Length > 0 && Enum.IsDefined(typeof(ChatCodeType), Data[0]))
                {
                    return (ChatCodeType)Data[0];
                }

                return ChatCodeType.Invalid;
            }
        }

        public byte[] Data { get; private set; }

        private ChatCode(byte[] data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return string.Format("[&{0}]", Convert.ToBase64String(Data));
        }

        #region Static Creation Methods
        public static ChatCode CreateCoinLink(uint c, uint s = 0, uint g = 0) { return CreateIntCode(ChatCodeType.Coin, (g * 10000) + (s * 100) + c); }

        public static ChatCode CreateItemLink(uint item_id, byte qty = 1)
        {
            byte[] tmp = CreateIntCode(ChatCodeType.Item, item_id).Data;

            byte[] data = new byte[tmp.Length + 1];
            data[0] = tmp[0];
            data[1] = qty;

            Array.Copy(tmp, 1, data, 2, tmp.Length - 1);

            return new ChatCode(data);
        }

        public static ChatCode CreateTextString(uint string_id) { return CreateIntCode(ChatCodeType.Text, string_id); }
        public static ChatCode CreateMapLink(uint poi_id) { return CreateIntCode(ChatCodeType.Map, poi_id); }
        public static ChatCode CreateSkillLink(uint skill_id) { return CreateIntCode(ChatCodeType.Skill, skill_id); }
        public static ChatCode CreateTraitLink(uint trait_id) { return CreateIntCode(ChatCodeType.Trait, trait_id); }
        public static ChatCode CreateRecipeLink(uint recipe_id) { return CreateIntCode(ChatCodeType.Recipe, recipe_id); }

        private static ChatCode CreateIntCode(ChatCodeType header, UInt32 id)
        {
            byte[] bytes = BitConverter.GetBytes(id);

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            byte[] data = new byte[bytes.Length + 1];

            data[0] = (byte)header;
            Array.Copy(bytes, 0, data, 1, bytes.Length);

            return new ChatCode(data);
        }
        #endregion
    }
}
