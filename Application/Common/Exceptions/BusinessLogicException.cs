using System;
using System.Collections.Generic;
using System.Linq;

namespace Biobrain.Application.Common.Exceptions
{
    public abstract class BusinessLogicException : Exception
    {
        public abstract Guid ErrorCode { get; }

        public List<BusinessLogicError> Errors { get; protected init; } = new();

        public override string Message
        {
            get
            {
                var error = $"{Environment.NewLine} Error code: {ErrorCode}";
                var values = Errors.Select(x => $"{Environment.NewLine} -- {x.Code}: {x.Description}");
                return $"Business logic failed:{error}{string.Join(string.Empty, values)}";
            }
        }
    }

    public record BusinessLogicError(string Code, string Description);
}