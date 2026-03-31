using System.Collections.Generic;
using System.Linq;
using Biobrain.Application.Payments.Models;
using Biobrain.Domain.Constants;
using Biobrain.Infrastructure.Payments.ErrorHandling;

namespace Biobrain.Infrastructure.Payments.Values
{
	public static class Prices
	{
		public  static double GetCost(int subjectsCount, PaymentPeriods period, string currency)
		{
			if (subjectsCount < 0 || subjectsCount > 6)
				throw new ScheduledPaymentException($"Subscriptions for {subjectsCount} courses not supported.");
			if (subjectsCount == 0) return 0;

			var value = Values.FirstOrDefault(x =>
				x.Period == period &&
				x.SubjectsNumber == subjectsCount &&
				x.Currency == currency)?.Value;

			return value ?? throw new ScheduledPaymentException("Price calculation error");
		}

		public static List<Price> Values { get; } = new()
		{
			// AUD
			// Month
			new Price { Currency = "AUD", SubjectsNumber = 1, Period = PaymentPeriods.Monthly, Value = 9.99, IsDisplayed = true},
			new Price { Currency = "AUD", SubjectsNumber = 2, Period = PaymentPeriods.Monthly, Value = 17.99, IsDisplayed = true },
			new Price { Currency = "AUD", SubjectsNumber = 3, Period = PaymentPeriods.Monthly, Value = 23.99, IsDisplayed = true },
            new Price { Currency = "AUD", SubjectsNumber = 4, Period = PaymentPeriods.Monthly, Value = 29.99, IsDisplayed = true },
            new Price { Currency = "AUD", SubjectsNumber = 5, Period = PaymentPeriods.Monthly, Value = 35.99, IsDisplayed = false },
            new Price { Currency = "AUD", SubjectsNumber = 6, Period = PaymentPeriods.Monthly, Value = 42.99, IsDisplayed = false },

			// Year
			new Price { Currency = "AUD", SubjectsNumber = 1, Period = PaymentPeriods.Yearly, Value = 69.99, IsDisplayed = true },
			new Price { Currency = "AUD", SubjectsNumber = 2, Period = PaymentPeriods.Yearly, Value = 109.99, IsDisplayed = true },
			new Price { Currency = "AUD", SubjectsNumber = 3, Period = PaymentPeriods.Yearly, Value = 139.99, IsDisplayed = true },
			new Price { Currency = "AUD", SubjectsNumber = 4, Period = PaymentPeriods.Yearly, Value = 149.99, IsDisplayed = true },
			new Price { Currency = "AUD", SubjectsNumber = 5, Period = PaymentPeriods.Yearly, Value = 179.99, IsDisplayed = false },
			new Price { Currency = "AUD", SubjectsNumber = 6, Period = PaymentPeriods.Yearly, Value = 214.99, IsDisplayed = false },


			// USD
			// Month
			new Price { Currency = "USD", SubjectsNumber = 1, Period = PaymentPeriods.Monthly, Value = 7.48, IsDisplayed = true },
			new Price { Currency = "USD", SubjectsNumber = 2, Period = PaymentPeriods.Monthly, Value = 13.46, IsDisplayed = true },
			new Price { Currency = "USD", SubjectsNumber = 3, Period = PaymentPeriods.Monthly, Value = 17.96, IsDisplayed = true },
			new Price { Currency = "USD", SubjectsNumber = 4, Period = PaymentPeriods.Monthly, Value = 22.45, IsDisplayed = true },
			new Price { Currency = "USD", SubjectsNumber = 5, Period = PaymentPeriods.Monthly, Value = 26.94, IsDisplayed = false },
			new Price { Currency = "USD", SubjectsNumber = 6, Period = PaymentPeriods.Monthly, Value = 32.18, IsDisplayed = false },

			// Year
			new Price { Currency = "USD", SubjectsNumber = 1, Period = PaymentPeriods.Yearly, Value = 49.99, IsDisplayed = true },
			new Price { Currency = "USD", SubjectsNumber = 2, Period = PaymentPeriods.Yearly, Value = 76.99, IsDisplayed = true },
			new Price { Currency = "USD", SubjectsNumber = 3, Period = PaymentPeriods.Yearly, Value = 99.99, IsDisplayed = true },
			new Price { Currency = "USD", SubjectsNumber = 4, Period = PaymentPeriods.Yearly, Value = 112.26, IsDisplayed = true },
			new Price { Currency = "USD", SubjectsNumber = 5, Period = PaymentPeriods.Yearly, Value = 134.71, IsDisplayed = false },
			new Price { Currency = "USD", SubjectsNumber = 6, Period = PaymentPeriods.Yearly, Value = 160.91, IsDisplayed = false },


			// EUR
			// Month
			new Price { Currency = "EUR", SubjectsNumber = 1, Period = PaymentPeriods.Monthly, Value = 6.43, IsDisplayed = true },
			new Price { Currency = "EUR", SubjectsNumber = 2, Period = PaymentPeriods.Monthly, Value = 11.58, IsDisplayed = true },
			new Price { Currency = "EUR", SubjectsNumber = 3, Period = PaymentPeriods.Monthly, Value = 15.44, IsDisplayed = true },
			new Price { Currency = "EUR", SubjectsNumber = 4, Period = PaymentPeriods.Monthly, Value = 19.31, IsDisplayed = true },
			new Price { Currency = "EUR", SubjectsNumber = 5, Period = PaymentPeriods.Monthly, Value = 23.17, IsDisplayed = false },
			new Price { Currency = "EUR", SubjectsNumber = 6, Period = PaymentPeriods.Monthly, Value = 27.67, IsDisplayed = false },

			// Year
			new Price { Currency = "EUR", SubjectsNumber = 1, Period = PaymentPeriods.Yearly, Value = 39.99, IsDisplayed = true },
			new Price { Currency = "EUR", SubjectsNumber = 2, Period = PaymentPeriods.Yearly, Value = 65.99, IsDisplayed = true },
			new Price { Currency = "EUR", SubjectsNumber = 3, Period = PaymentPeriods.Yearly, Value = 87.99, IsDisplayed = true },
			new Price { Currency = "EUR", SubjectsNumber = 4, Period = PaymentPeriods.Yearly, Value = 96.55, IsDisplayed = true },
			new Price { Currency = "EUR", SubjectsNumber = 5, Period = PaymentPeriods.Yearly, Value = 115.86, IsDisplayed = false },
			new Price { Currency = "EUR", SubjectsNumber = 6, Period = PaymentPeriods.Yearly, Value = 138.40, IsDisplayed = false },


			// GBR
			// Month
			new Price { Currency = "GBP", SubjectsNumber = 1, Period = PaymentPeriods.Monthly, Value = 5.43, IsDisplayed = true },
			new Price { Currency = "GBP", SubjectsNumber = 2, Period = PaymentPeriods.Monthly, Value = 9.78, IsDisplayed = true },
			new Price { Currency = "GBP", SubjectsNumber = 3, Period = PaymentPeriods.Monthly, Value = 13.04, IsDisplayed = true },
			new Price { Currency = "GBP", SubjectsNumber = 4, Period = PaymentPeriods.Monthly, Value = 16.30, IsDisplayed = true },
			new Price { Currency = "GBP", SubjectsNumber = 5, Period = PaymentPeriods.Monthly, Value = 19.56, IsDisplayed = false },
			new Price { Currency = "GBP", SubjectsNumber = 6, Period = PaymentPeriods.Monthly, Value = 23.37, IsDisplayed = false },

			// Year
			new Price { Currency = "GBP", SubjectsNumber = 1, Period = PaymentPeriods.Yearly, Value = 27.17, IsDisplayed = true },
			new Price { Currency = "GBP", SubjectsNumber = 2, Period = PaymentPeriods.Yearly, Value = 48.91, IsDisplayed = true },
			new Price { Currency = "GBP", SubjectsNumber = 3, Period = PaymentPeriods.Yearly, Value = 65.22, IsDisplayed = true },
			new Price { Currency = "GBP", SubjectsNumber = 4, Period = PaymentPeriods.Yearly, Value = 81.52, IsDisplayed = true },
			new Price { Currency = "GBP", SubjectsNumber = 5, Period = PaymentPeriods.Yearly, Value = 97.83, IsDisplayed = false },
			new Price { Currency = "GBP", SubjectsNumber = 6, Period = PaymentPeriods.Yearly, Value = 116.85, IsDisplayed = false },
		};
	}
}