using System;
using System.Linq;
using Biobrain.Application.Specifications;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Common.Validators;

public sealed class UniqueValidator<TContext, TProperty, TEntity> : PropertyValidator<TContext, TProperty> where TEntity : class
{
    /// <inheritdoc />
    public override string Name => nameof(UniqueValidator<TContext, TProperty, TEntity>);

    public UniqueValidator(DbSet<TEntity> dbSet, Func<TProperty, Spec<TEntity>> spec)
    {
        _dbSet = dbSet;
        _spec = spec;
    }

    public override bool IsValid(ValidationContext<TContext> context, TProperty value)
    {
        context.MessageFormatter.AppendArgument("ObjectType", typeof(TEntity).Name);
        context.MessageFormatter.AppendArgument("Value", value);

        return !_dbSet.Any(_spec(value));
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' is not unique. (value ='{Value}').";

    private readonly DbSet<TEntity> _dbSet;
    private readonly Func<TProperty, Spec<TEntity>> _spec;
}
