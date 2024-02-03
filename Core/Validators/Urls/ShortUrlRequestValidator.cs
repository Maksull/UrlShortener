using Core.Contracts.UrlEndpoints;
using FluentValidation;

namespace Core.Validators.Urls;

public sealed class ShortUrlRequestValidator : AbstractValidator<ShortUrlRequest>
{
    public ShortUrlRequestValidator()
    {
        RuleFor(o => o.LongUrl).IsValidUri();
    }
}
