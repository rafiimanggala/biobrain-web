using System;
using Biobrain.Application.Common.Exceptions;
using JetBrains.Annotations;

namespace Biobrain.Application.Accounts.Exceptions
{
    [PublicAPI]
    public class UserHasNoEmailException : BusinessLogicException
    {
        public override Guid ErrorCode => new("79E49B86-D3A3-4E9C-894D-95EED7C2A3C6");
    }
}