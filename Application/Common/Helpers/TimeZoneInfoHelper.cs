using System;
using TimeZoneConverter;

namespace Biobrain.Application.Common.Helpers
{
	public static class TimeZoneInfoHelper
	{
		public static DateTime ConvertTimeFromUtc(DateTime utcDateTime, string timezoneId)
		{
			var timeZone = TZConvert.GetTimeZoneInfo(timezoneId);
			return System.TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
		}
		public static DateTime ConvertTimeToUtc(DateTime utcDateTime, string timezoneId)
		{
			var timeZone = TZConvert.GetTimeZoneInfo(timezoneId);
			return System.TimeZoneInfo.ConvertTimeToUtc(utcDateTime, timeZone);
		}
	}
}