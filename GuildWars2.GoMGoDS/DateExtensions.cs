using System;

namespace GuildWars2.GoMGoDS
{
    public static class DateExtensions
    {
        public static TimeSpan Round(this TimeSpan span, TimeSpan roundingInterval, MidpointRounding roundingType = MidpointRounding.ToEven)
        {
            return new TimeSpan(
                    Convert.ToInt64(Math.Round(
                            span.Ticks / (decimal)roundingInterval.Ticks,
                            roundingType
                        )) * roundingInterval.Ticks
                );
        }

        public static TimeSpan Ceiling(this TimeSpan span, TimeSpan roundingInterval)
        {
            return new TimeSpan(
                    Convert.ToInt64(Math.Ceiling(
                            span.Ticks / (decimal)roundingInterval.Ticks
                        )) * roundingInterval.Ticks
                );
        }

        public static TimeSpan Floor(this TimeSpan span, TimeSpan roundingInterval)
        {
            return new TimeSpan(
                    Convert.ToInt64(Math.Floor(
                            span.Ticks / (decimal)roundingInterval.Ticks
                        )) * roundingInterval.Ticks
                );
        }

        public static DateTime Round(this DateTime time, TimeSpan roundingInterval)
        {
            return new DateTime(
                    (time - DateTime.MinValue).Round(roundingInterval).Ticks,
                    time.Kind
                );
        }

        public static DateTime Ceiling(this DateTime time, TimeSpan roundingInterval)
        {
            return new DateTime(
                    (time - DateTime.MinValue).Ceiling(roundingInterval).Ticks,
                    time.Kind
                );
        }

        public static DateTime Floor(this DateTime time, TimeSpan roundingInterval)
        {
            return new DateTime(
                    (time - DateTime.MinValue).Floor(roundingInterval).Ticks,
                    time.Kind
                );
        }
    }
}
