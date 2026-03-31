using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Biobrain.Application.Common.Validators;

public sealed class GuidValidator<TContext> : PropertyValidator<TContext, string>
{
    /// <inheritdoc />
    public override string Name => nameof(GuidValidator<TContext>);

    public override bool IsValid(ValidationContext<TContext> context, string value) => value == null || Guid.TryParse(value, out _);
    protected override string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' is not a valid GUID.";
}
