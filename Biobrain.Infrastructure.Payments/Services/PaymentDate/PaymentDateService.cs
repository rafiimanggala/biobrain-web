using System;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Domain.Constants;
using TimeZoneConverter;

namespace Biobrain.Infrastructure.Payments.Services.PaymentDate
{
    public class PaymentDateService: IPaymentDateService
    {
        private const int LeapYear = 2000;
        private const int NotLeapYear = 2001;
        //private const int PaymentLocalHour = 12;

        /// <summary>
        /// Convert pay date to int format for leap year that more selectable and easier to use.
        /// </summary>
        /// <param name="utcDate">Date of pay utc</param>
        /// <returns>
        /// Pay mark for leap year that has format mmddhh in utc
        /// mm - month (01)
        /// dd - day (01)
        /// hh - hour (12)
        /// </returns>
        public int GetLeapPaydate(DateTime utcDate) => GetPaydate(GetLeapPayDateTime(utcDate));

        /// <summary>
        /// Convert pay date to int format for not leap year that more selectable and easier to use.
        /// </summary>
        /// <param name="utcDate">Date of pay utc</param>
        /// <returns>
        /// Pay mark for not leap year that has format mmddhh in utc
        /// mm - month (01)
        /// dd - day (01)
        /// hh - hour (12)
        /// </returns>
        public int GetNotLeapPaydate(DateTime utcDate) => GetPaydate(GetNotLeapPayDateTime(utcDate));

        /// <summary>
        /// Convert pay date to int format that more selectable and easier to use.
        /// </summary>
        /// <param name="utcDate">Date of pay utc</param>
        /// <returns>
        /// Pay mark that has format mmddhh in utc
        /// mm - month (01)
        /// dd - day (01)
        /// hh - hour (12)
        /// </returns>
        public int GetPaydate(DateTime utcDate) =>
            //Convert to int format: mmddhh (m-month, d - day, h - hour)
            utcDate.Month * 10000 + utcDate.Day * 100 + utcDate.Hour;

        private static DateTime GetNotLeapPayDateTime(DateTime utcPayDate) => DateTime.SpecifyKind(
            new DateTime(NotLeapYear, utcPayDate.Month,
                utcPayDate.Month == 2 && utcPayDate.Day == 29 ? 28 : utcPayDate.Day).Date.AddHours(utcPayDate.Hour), DateTimeKind.Unspecified);

        private static DateTime GetLeapPayDateTime(DateTime payDateTime) => DateTime.SpecifyKind(
            new DateTime(LeapYear, payDateTime.Month, payDateTime.Day).Date
                .AddHours(payDateTime.Hour), DateTimeKind.Unspecified);

        /// <summary>
        /// Get next pay date from format mmddhh in local timezone.
        /// </summary>
        /// <param name="notLeapPayDate"></param>
        /// <param name="leapPayDate"></param>
        /// <param name="timezoneId"></param>
        /// <returns>
        /// Next pay date
        /// </returns>
        public DateTime GetNextPayDateLocal(int notLeapPayDate, int leapPayDate, string timezoneId)
        {
            var dateNow = GetPaydate(DateTime.UtcNow);

            //Different current pay date if leap year to handle 29 Feb
            var currentPayDate = DateTime.IsLeapYear(DateTime.UtcNow.Year) ? leapPayDate : notLeapPayDate;

            //If there is a payment in current year than next payment in next year
            var year = currentPayDate > dateNow ? DateTime.UtcNow.Year : DateTime.UtcNow.Year + 1;
            //Different next pay date if leap year to handle 29 Feb
            var payDate = DateTime.IsLeapYear(year) ? leapPayDate : notLeapPayDate;
            var month = (int)Math.Truncate((double)payDate / 10000);
            var day = (int)Math.Truncate((double)payDate / 100) % 100;
            var hour = (int)((double)payDate % 100);

            var nextPayDateUtc = new DateTime(year, month, day, hour, 0, 0);
            var timeZone = TZConvert.GetTimeZoneInfo(timezoneId);

            return TimeZoneInfo.ConvertTimeFromUtc(nextPayDateUtc, timeZone);
        }

        /// <summary>
        /// Get next pay date
        /// </summary>
        /// <param name="notLeapPayDate"></param>
        /// <param name="leapPayDate"></param>
        /// <param name="period"></param>
        /// <returns>
        /// Next pay date
        /// </returns>
        public DateTime GetNextPayDateUtc(int notLeapPayDate, int leapPayDate, PaymentPeriods period)
        {
            return period switch
            {
                PaymentPeriods.Monthly => GetNextPayDateUtcForMonth(notLeapPayDate, leapPayDate),
                PaymentPeriods.Yearly => GetNextPayDateUtcForYear(notLeapPayDate, leapPayDate),
                _ => throw new ArgumentOutOfRangeException(nameof(period), period, null)
            };
        }

        private DateTime GetNextPayDateUtcForYear(int notLeapPayDate, int leapPayDate)
        {
	        var dateNow = GetPaydate(DateTime.UtcNow);

	        //Different current pay date if leap year to handle 29 Feb
	        var currentPayDate = DateTime.IsLeapYear(DateTime.UtcNow.Year) ? leapPayDate : notLeapPayDate;

	        //If there is a payment in current year than next payment in next year
	        var year = currentPayDate > dateNow ? DateTime.UtcNow.Year : DateTime.UtcNow.Year + 1;
	        //Different next pay date if leap year to handle 29 Feb
	        var payDate = DateTime.IsLeapYear(year) ? leapPayDate : notLeapPayDate;
	        var month = (int)Math.Truncate((double)payDate / 10000);
	        var day = (int)Math.Truncate((double)payDate / 100) % 100;
	        var hour = (int)((double)payDate % 100);

	        var nextPayDateUtc = new DateTime(year, month, day, hour, 0, 0);

	        return nextPayDateUtc;
        }

        private DateTime GetNextPayDateUtcForMonth(int notLeapPayDate, int leapPayDate)
        {
	        var dateNow = GetPaydate(DateTime.UtcNow) % 10000;

	        //Different current pay date if leap year to handle 29 Feb
	        var currentPayDate = (DateTime.IsLeapYear(DateTime.UtcNow.Year) ? leapPayDate : notLeapPayDate) % 10000;

            //DateTime with month and year of next payment
            var monthAndYearDateTime = currentPayDate > dateNow ? DateTime.UtcNow : DateTime.UtcNow.AddMonths(1);
            var year = monthAndYearDateTime.Year;
            var month = monthAndYearDateTime.Month;
            //Different next pay date if leap year to handle 29 Feb
            var payDate = (DateTime.IsLeapYear(year) ? leapPayDate : notLeapPayDate) % 10000;
	        var day = (int)Math.Truncate((double)payDate / 100) % 100;
	        var hour = (int)((double)payDate % 100);

	        var nextPayDateUtc = new DateTime(year, month, day, hour, 0, 0);

	        return nextPayDateUtc;
        }

        /// <summary>
        /// Return DateTime.UtcNow converted to specific timezone
        /// </summary>
        /// <param name="timezoneId">Timezone to convert</param>
        /// <returns>UtcNow in specific timezone</returns>
        public DateTime GetNowInCustomTZ(string timezoneId)
        {
            var timeZone = TZConvert.GetTimeZoneInfo(timezoneId);
            var date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            return date;
        }
        
    }
}