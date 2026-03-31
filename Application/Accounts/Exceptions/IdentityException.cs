using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Application.Common.Exceptions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Biobrain.Application.Accounts.Exceptions
{
    [PublicAPI]
    public abstract class IdentityException : BusinessLogicException
    {
        protected IdentityException(IEnumerable<IdentityError> errors)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            Errors = errors.Select(e => new BusinessLogicError(e.Code, e.Description)).ToList();
        }
    }
}
