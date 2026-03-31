using System;

namespace BiobrainWebAPI.Core.ErrorHandling.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message)
        {
        }
    }
}