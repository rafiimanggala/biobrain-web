using System;
using Biobrain.Application.Common.Exceptions;
using JetBrains.Annotations;

namespace Biobrain.Application.Accounts.Exceptions
{
    [PublicAPI]
    public class IncorrectPasswordException : BusinessLogicException
    {
        public override Guid ErrorCode => new("46A82F7B-AF62-4511-B8EB-6066730338BA");
    }
}