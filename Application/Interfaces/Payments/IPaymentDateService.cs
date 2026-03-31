using System;
using Biobrain.Domain.Constants;

namespace Biobrain.Application.Interfaces.Payments
{
    public interface IPaymentDateService
    {
        /// <summary>
        /// Convert pay date to int format that more selectable and easier to use.
        /// </summary>
        /// <param name="date">Date of pay (utc)</param>
        /// <returns>
        /// Pay mark that has format mmddhh in utc
        /// mm - month (01)
        /// dd - day (01)
        /// hh - hour (12)
        /// </returns>
        int GetPaydate(DateTime date);

        /// <summary>
        /// Convert pay date to int format for leap year that more selectable and easier to use.
        /// </summary>
        /// <param name="date">Date of pay (utc)</param>
        /// <returns>
        /// Pay mark for leap year that has format mmddhh in utc
        /// mm - month (01)
        /// dd - day (01)
        /// hh - hour (12)
        /// </returns>
        int GetLeapPaydate(DateTime date);

        /// <summary>
        /// Convert pay date to int format for not leap year that more selectable and easier to use.
        /// </summary>
        /// <param name="date">Date of pay (utc)</param>
        /// <returns>
        /// Pay mark for not leap year that has format mmddhh in utc
        /// mm - month (01)
        /// dd - day (01)
        /// hh - hour (12)
        /// </returns>
        int GetNotLeapPaydate(DateTime date);

        /// <summary>
        /// Get next pay date from format mmddhh in local time.
        /// </summary>
        /// <param name="notLeapPayDate"></param>
        /// <param name="leapPayDate"></param>
        /// <param name="timezoneId"></param>
        /// <returns>
        /// Next pay date
        /// </returns>
        DateTime GetNextPayDateLocal(int notLeapPayDate, int leapPayDate, string timezoneId);

        /// <summary>
        /// Get next pay date
        /// </summary>
        /// <param name="notLeapPayDate"></param>
        /// <param name="leapPayDate"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        DateTime GetNextPayDateUtc(int notLeapPayDate, int leapPayDate, PaymentPeriods period);

        /// <summary>
        /// Return DateTime.UtcNow converted to specific timezone
        /// </summary>
        /// <param name="timezoneId">Timezone to convert</param>
        /// <returns>UtcNow in specific timezone</returns>
        DateTime GetNowInCustomTZ(string timezoneId);
    }
}