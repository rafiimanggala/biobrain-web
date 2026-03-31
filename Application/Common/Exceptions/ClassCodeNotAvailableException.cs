using System;

namespace Biobrain.Application.Common.Exceptions
{
    public class ClassCodeNotAvailableException : BusinessLogicException
    {
        public override Guid ErrorCode => new Guid("6118A9D0-1BDA-4C6D-B15B-717A6217A531");
    }
}