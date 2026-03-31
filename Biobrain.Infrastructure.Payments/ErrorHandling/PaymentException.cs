using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace Biobrain.Infrastructure.Payments.ErrorHandling
{
	public class PaymentException : ValidationException
	{

		public PaymentException(string message) : base(message) { }

		public PaymentException(string message, IEnumerable<ValidationFailure> errors) : base(message, errors) { }
	}
}