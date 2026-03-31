using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Biobrain.Application.Common.Validators;

public class PhoneNumberValidator<TContext> : PropertyValidator<TContext, string>
{
    private readonly Regex _regex = new("^(\\+7|7|8)+\\d{10}$");

    protected override string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' is not a valid phone number.";

    /// <inheritdoc />
    public override string Name => nameof(PhoneNumberValidator<TContext>);

    public override bool IsValid(ValidationContext<TContext> validationContext, string property) => property == null || _regex.IsMatch(property);
}