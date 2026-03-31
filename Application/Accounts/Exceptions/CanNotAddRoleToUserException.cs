using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Accounts.Exceptions
{
    [PublicAPI]
    public class CanNotAddRoleToUserException : IdentityException
    {
        public override Guid ErrorCode => new("D62D3349-8C5C-4DB5-BD9F-EDB3A6E45DC2");

        public CanNotAddRoleToUserException(IEnumerable<IdentityError> errors) : base(errors)
        {
        }
    }
}