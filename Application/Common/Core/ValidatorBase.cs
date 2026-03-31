using Biobrain.Application.Interfaces.DataAccess;
using FluentValidation;

namespace Biobrain.Application.Common.Core
{
    public abstract class ValidatorBase<TRequest> : AbstractValidator<TRequest>
    {
        protected readonly IDb Db;

        protected ValidatorBase(IDb db) => Db = db;
    }
}