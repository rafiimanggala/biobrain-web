using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Accounts.Exceptions
{
    public class CanNotChangePasswordException : IdentityException
    {
        public override Guid ErrorCode => new("ABAD31BE-6334-492B-9EFE-B43F961207C0");

        public CanNotChangePasswordException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}