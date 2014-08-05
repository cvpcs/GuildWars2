using System;
using System.Collections.Generic;

namespace GuildWars2.GoMGoDS.Model
{
    public static class MetaEventSchedule
    {
        // these run at 15 and 45 minutes past the hour unless an extra hard boss is running (slots 1, 3)
        private static string[] ME_ROTATION_LOW_LEVEL = new string[]
            {
                MetaEventDefinitions.MEID_WURM,
                MetaEventDefinitions.MEID_BEHEMOTH,
                MetaEventDefinitions.MEID_MAW,
                MetaEventDefinitions.MEID_ELEMENTAL
            };

        // these run on and 30 minutes past the hour unless an extra hard boss is running (slots 0, 2)
        private static string[] ME_ROTATION_STANDARD = new string[]
            {
                MetaEventDefinitions.MEID_SHATTERER,
                MetaEventDefinitions.MEID_ULGOTH,
                MetaEventDefinitions.MEID_GOLEM,
                MetaEventDefinitions.MEID_CLAW,
                MetaEventDefinitions.MEID_TAIDHA,
                MetaEventDefinitions.MEID_MEGADESTROYER,
                null
            };

        // these run at specific times
        private static IDictionary<int, string> ME_ROTATION_EXTRA_HARD_SLOTS = new Dictionary<int, string>
            {
                {  3 * 4 + 2, MetaEventDefinitions.MEID_KARKA   }, //  3:30
                {  4 * 4 + 2, MetaEventDefinitions.MEID_TEQUATL }, //  4:30
                {  5 * 4 + 2, MetaEventDefinitions.MEID_TRIWURM }, //  5:30
                {  9 * 4 + 0, MetaEventDefinitions.MEID_KARKA   }, //  9:00
                { 10 * 4 + 0, MetaEventDefinitions.MEID_TEQUATL }, // 10:00
                { 11 * 4 + 0, MetaEventDefinitions.MEID_TRIWURM }, // 11:00
                { 18 * 4 + 0, MetaEventDefinitions.MEID_KARKA   }, // 18:00
                { 19 * 4 + 0, MetaEventDefinitions.MEID_TEQUATL }, // 19:00
                { 20 * 4 + 0, MetaEventDefinitions.MEID_TRIWURM }  // 20:00
            };

        private static TimeZoneInfo PST8PDT;

        public static string[] MetaEventRotation;
        public static HashSet<string> MetaEventList;

        static MetaEventSchedule()
        {
            // there are 96 15-minute slots in a day for the rotation
            MetaEventRotation = new string[24 * 4];

            for (int i = 0, j = 0, k = 0; i < MetaEventRotation.Length; i++)
            {
                if (ME_ROTATION_EXTRA_HARD_SLOTS.ContainsKey(i))
                    MetaEventRotation[i] = ME_ROTATION_EXTRA_HARD_SLOTS[i];
                else if (i % 2 == 0)
                    MetaEventRotation[i] = ME_ROTATION_STANDARD[j++ % ME_ROTATION_STANDARD.Length];
                else
                    MetaEventRotation[i] = ME_ROTATION_LOW_LEVEL[k++ % ME_ROTATION_LOW_LEVEL.Length];
            }

            // create a list of all of the metas that are scheduled
            MetaEventList = new HashSet<string>(MetaEventRotation);

            PST8PDT = TimeZoneInfo.FindSystemTimeZoneById(Environment.OSVersion.Platform == PlatformID.Unix ? "PST8PDT" : "Pacific Standard Time");
        }

        public static int GetSlot(DateTime utcTime)
        {
            DateTime pt = TimeZoneInfo.ConvertTimeFromUtc(utcTime, PST8PDT);
            return (int)Math.Floor(pt.TimeOfDay.TotalMinutes / 15.0) % MetaEventRotation.Length;
        }
    }
}
