using System;
using System.Net;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace PinPayments
{
    public class PinException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; protected set; }
        public PinError Error { get; protected set; }

        public PinException(HttpStatusCode httpStatusCode, PinError error, string message)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
            Error = error;
        }
    }
}
