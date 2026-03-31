using Biobrain.Domain.Constants;

namespace Biobrain.Application.Payments.Models
{
	public class Price
	{
		public string Currency { get; set; }

		/// <summary>
		/// Number of subjects
		/// </summary>
		public int SubjectsNumber { get; set; }

		/// <summary>
		/// Payment period
		/// </summary>
		public PaymentPeriods Period { get; set; }

		/// <summary>
		/// Price value
		/// </summary>
		public double Value { get; set; }

        /// <summary>
        /// Is displayed in interface
        /// </summary>
        public bool IsDisplayed { get; set; }
    }
}