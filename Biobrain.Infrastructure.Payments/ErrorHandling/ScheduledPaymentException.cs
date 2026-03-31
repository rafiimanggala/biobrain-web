using System;

namespace Biobrain.Infrastructure.Payments.ErrorHandling
{
	public class ScheduledPaymentException : Exception
	{
		public ScheduledPaymentException(string message): base(message){}
	}
}