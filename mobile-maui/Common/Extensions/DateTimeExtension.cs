using System;

namespace BioBrain.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime ConvertFromUnixTimestamp(this int timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static int ConvertToUnixTimestamp(this DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = date.ToUniversalTime() - origin;
            return (int)Math.Floor(diff.TotalSeconds);
        }
    }
}