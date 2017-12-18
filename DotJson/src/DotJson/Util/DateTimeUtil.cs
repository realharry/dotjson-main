using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Util
{
    public static class DateTimeExtension
    {
        // Note: All DateTime args are based on local time.

        // "Epoch".
        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToLocalDateTime(this long epochMillis)
        {
            // return epochMillis.ToLocalDateTime(0L);
            return EPOCH.AddMilliseconds(epochMillis).ToLocalTime();
        }
        public static DateTime ToLocalDateTime(this long epochMillis, long delta)
        {
            return EPOCH.AddMilliseconds(epochMillis + delta).ToLocalTime();
        }

        public static DateTime ToLocalDateTime(this ulong epochMillis)
        {
            // return epochMillis.ToLocalDateTime(0UL);
            return EPOCH.AddMilliseconds(epochMillis).ToLocalTime();
        }
        public static DateTime ToLocalDateTime(this ulong epochMillis, ulong delta)
        {
            return EPOCH.AddMilliseconds(epochMillis + delta).ToLocalTime();
        }


        public static TimeSpan ToTimeSpan(this ulong millis)
        {
            int seconds = (int) (millis / 1000UL);
            int milliseconds = (int) (millis % 1000UL);
            return new TimeSpan(0, 0, 0, seconds, milliseconds);
        }


        public static long ToUnixEpochMillis(this DateTime dateTime)
        {
            return Convert.ToInt64((dateTime.ToUniversalTime() - EPOCH).TotalMilliseconds);
        }

        public static long ToTotalMilliseconds(this TimeSpan timeSpan)
        {
            // return timeSpan.TotalMilliseconds;
            return (timeSpan.Ticks / TimeSpan.TicksPerMillisecond);
        }
        public static int ToTotalSeconds(this TimeSpan timeSpan)
        {
            // return timeSpan.TotalSeconds;
            return (int) (timeSpan.Ticks / TimeSpan.TicksPerSecond);
        }

    }

    public static class DateTimeUtil
    {
        public static long CurrentUnixEpochMillis()
        {
            return DateTime.Now.ToUnixEpochMillis();
        }


        // temporary
        public static long GetMidnight(long now)
        {
            DateTime date = now.ToLocalDateTime();
            long midnight = date.Date.ToUnixEpochMillis();
            return midnight;
        }

        // temporary
        public static long GetSundayMidnight(long now)
        {
            DateTime date = now.ToLocalDateTime();
            int delta = 0 - date.DayOfWeek;
            date.AddDays((double) delta);
            long sundayMidnight = date.Date.ToUnixEpochMillis();
            return sundayMidnight;
        }

        // temporary
        public static long GetFirstDayMidnight(long now)
        {
            DateTime date = now.ToLocalDateTime();
            int delta = 0 - date.Day;
            date.AddDays((double) delta);
            long sundayMidnight = date.Date.ToUnixEpochMillis();
            return sundayMidnight;
        }

        // temporary
        public static uint GetNumberOfDaysForMonth(long now)
        {
            DateTime date = now.ToLocalDateTime();
            var numDays = 0U;
            switch (date.Month) {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    numDays = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                default:   // ???
                    numDays = 30;
                    break;
                case 2:
                    numDays = 28;
                    if ((date.Year % 4) == 0) {
                        numDays = 29;
                    }
                    break;
            }
            return numDays;
        }


    }
}
