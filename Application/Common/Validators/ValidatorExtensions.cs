using System;
using Biobrain.Application.Specifications;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Common.Validators;

internal static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> Guid<T>(this IRuleBuilder<T, string> ruleBuilder) => ruleBuilder.SetValidator(new GuidValidator<T>());

    public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder) => ruleBuilder.SetValidator(new PhoneNumberValidator<T>());

    internal static IRuleBuilderOptions<TContext, TProperty> ExistsInTable<TContext, TProperty, TEntity>(
        this IRuleBuilder<TContext, TProperty> ruleBuilder,
        DbSet<TEntity> dbSet
    )
        where TEntity : class
        => ruleBuilder.SetValidator(new ExistsInTableByIdValidator<TContext, TProperty, TEntity>(dbSet));

    internal static IRuleBuilderOptions<TContext, TProperty> ExistsInTable<TContext, TProperty, TEntity>(
        this IRuleBuilder<TContext, TProperty> ruleBuilder,
        DbSet<TEntity> dbSet,
        Func<TProperty, Spec<TEntity>> spec
    )
        where TEntity : class
        => ruleBuilder.SetValidator(new ExistsInTableBySpecValidator<TContext, TProperty, TEntity>(dbSet, spec));

    internal static IRuleBuilderOptions<TContext, TProperty> Unique<TContext, TProperty, TEntity>(
        this IRuleBuilder<TContext, TProperty> ruleBuilder,
        DbSet<TEntity> dbSet,
        Func<TProperty, Spec<TEntity>> spec
    )
        where TEntity : class
        => ruleBuilder.SetValidator(new UniqueValidator<TContext, TProperty, TEntity>(dbSet, spec));
}
