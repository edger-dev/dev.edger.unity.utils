using System;
using System.Globalization;

namespace Edger.Unity {
    public static class DateTimeUtil {
        public static readonly string TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss";

        public static string UtcTimestamp {
            get {
                return DateTime.UtcNow.ToString(TIMESTAMP_FORMAT);
            }
        }

        public static string UtcGuid {
            get {
                return DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_") + Guid.NewGuid().ToString();
            }
        }

        public static string GetTimestamp(DateTime theDate) {
            return theDate.ToString(TIMESTAMP_FORMAT);
        }

        public static long UnixSeconds {
            get {
                return ToUnixSeconds(DateTime.UtcNow);
            }
        }

        public static long UnixMilliseconds {
            get {
                return ToUnixMilliseconds(DateTime.UtcNow);
            }
        }

        public static long UnixNanoseconds {
            get {
                return ToUnixNanoseconds(DateTime.UtcNow);
            }
        }

        public static long ToUnixSeconds(DateTime theDate)
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            return (long)t.TotalSeconds;
        }

        public static long ToUnixMilliseconds(DateTime theDate)
        {
            TimeSpan t = (theDate - new DateTime(1970, 1, 1));
            return (long)t.TotalMilliseconds;
        }

        public static long ToUnixNanoseconds(DateTime theDate)
        {
            TimeSpan t = (theDate - new DateTime(1970, 1, 1));
            return (long)t.Ticks * 100;
        }

        public static DateTime FromUnixSeconds(long unixSeconds) {
            DateTime unixStartTime = new DateTime(1970, 1, 1);
            return unixStartTime.AddSeconds(unixSeconds);
        }

        public static DateTime FromUnixMilliseconds(long unixMilliseconds) {
            DateTime unixStartTime = new DateTime(1970, 1, 1);
            return unixStartTime.AddMilliseconds(unixMilliseconds);
        }

        public static string GetTimestampFromUnixSeconds(long unixSeconds) {
            return FromUnixSeconds(unixSeconds).ToString(TIMESTAMP_FORMAT);
        }

        public static string GetTimestampFromUnixMilliseconds(long unixMilliseconds) {
            return FromUnixMilliseconds(unixMilliseconds).ToString(TIMESTAMP_FORMAT);
        }

        public static string GetExpireTimestamp(long expireSeconds) {
            return DateTime.UtcNow.AddSeconds(expireSeconds).ToString(TIMESTAMP_FORMAT);
        }

        public static DateTime GetDateTimeFromTimestamp(string timestamp) {
            return DateTime.ParseExact(timestamp, TIMESTAMP_FORMAT, CultureInfo.InvariantCulture);
        }

        public static int CompareTimestamp(string a, string b) {
            return string.CompareOrdinal(a, b);
        }

        public static DateTime GetStartTime() {
            return DateTime.UtcNow;
        }

        public static double GetPassedSeconds(this DateTime startTime) {
            return GetPassedSeconds(ref startTime, false);
        }

        public static double GetPassedSeconds(ref DateTime startTime, bool updateToNow = true) {
            DateTime newStartTime = DateTime.UtcNow;
            TimeSpan duration = newStartTime - startTime;
            if (updateToNow) {
                startTime = newStartTime;
            }
            return duration.TotalSeconds;
        }

        public static long GetUtcNowTicks() {
            return System.DateTime.UtcNow.Ticks;
        }

        public static string GetTimestampFromTicks(long ticks) {
            return new DateTime(ticks, DateTimeKind.Utc).ToString(TIMESTAMP_FORMAT);
        }

        public static float TicksToSeconds(long ticks) {
            long seconds = ticks / TimeSpan.TicksPerSecond;
            long subSecond = ticks % TimeSpan.TicksPerSecond;
            return (float)seconds + (float)subSecond / TimeSpan.TicksPerSecond;
        }

        public static long SecondsToTicks(float seconds) {
            return (long)(seconds * TimeSpan.TicksPerSecond);
        }

        public static float GetSecondsFrom(this System.DateTime dateTime, long baseTicks) {
            return TicksToSeconds(dateTime.Ticks - baseTicks);
        }

        public static float GetSecondsTo(this System.DateTime dateTime, long timeoutTicks) {
            return TicksToSeconds(timeoutTicks - dateTime.Ticks);
        }
    }
}
