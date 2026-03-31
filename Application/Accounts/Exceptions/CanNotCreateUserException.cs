using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Accounts.Exceptions
{
    [PublicAPI]
    public class CanNotCreateUserException : IdentityException
    {
        public override Guid ErrorCode => new("E8BB101E-E78F-4702-9D92-04A8BA4F07F5");

        public CanNotCreateUserException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}