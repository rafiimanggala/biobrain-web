using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Common.Validators;

public class ExistsInTableByIdValidator<TContext, TProperty, TEntity>(DbSet<TEntity> dbSet) : PropertyValidator<TContext, TProperty>
    where TEntity : class
{
    /// <inheritdoc />
    public override string Name => nameof(ExistsInTableByIdValidator<TContext, TProperty, TEntity>);

    public override bool IsValid(ValidationContext<TContext> context, TProperty property)
    {
        TProperty id = property;

        context.MessageFormatter.AppendArgument("ObjectType", typeof(TEntity).Name);
        context.MessageFormatter.AppendArgument("Value", id);

        if (id == null)
            return false;

        TEntity entity = DbSet.Find(id);
        return entity != null;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "'{ObjectType}' with '{PropertyName}' = '{Value}' was not found.";

    private DbSet<TEntity> DbSet { get; } = dbSet;
}
