using System;
using System.Linq.Expressions;

namespace Biobrain.Application.Specifications
{
    public class Spec<T>
    {
        public static implicit operator Expression<Func<T, bool>>(Spec<T> spec) => spec.Expression;

        private Expression<Func<T, bool>> Expression { get; }

        public Spec(Expression<Func<T, bool>> expression) => Expression = expression;

        //TODO: compiled expressions should be cached because compilation is slow operation
        public bool IsSatisfiedBy(T value) => Expression.Compile()(value);
    }
}