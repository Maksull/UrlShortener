using FluentValidation;

namespace Core.Validators;

internal static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> IsValidUri<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(uriString =>
            {
                return Uri.TryCreate(uriString, UriKind.Absolute, out _);
            })
            .WithMessage("Invalid URI format");
    }
}
