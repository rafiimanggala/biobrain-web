using System;
using System.Collections.Generic;
using Biobrain.Application.Accounts.Exceptions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Accounts.ResetPassword
{
    [PublicAPI]
    public class CanNotSetPasswordException : IdentityException
    {
        public override Guid ErrorCode => new("BAE163A0-A30E-4132-9BD1-EF1B1ADFED6C");

        public CanNotSetPasswordException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}