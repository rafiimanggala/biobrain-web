using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Accounts.Exceptions
{
    [PublicAPI]
    public class CanNotChangeUserEmailException : IdentityException
    {
        public override Guid ErrorCode => new("027A3A53-E00A-4A8F-B0ED-B3F1AA5DCDE7");

        public CanNotChangeUserEmailException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}