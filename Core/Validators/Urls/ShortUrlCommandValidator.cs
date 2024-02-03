using Core.Mediatr.Commands;
using FluentValidation;

namespace Core.Validators.Urls;

public sealed class ShortUrlCommandValidator : AbstractValidator<ShortUrlCommand>
{
    public ShortUrlCommandValidator()
    {
        RuleFor(o => o.AppUrl).IsValidUri();
        RuleFor(o => o.LongUrl).IsValidUri();
    }
}
