using Core.Contracts.UrlEndpoints;
using Core.Validators.Urls;

namespace Core.Tests.Validators;

public sealed class ShortUrlRequestValidatorTests
{
    [Fact]
    public void Validate_WhenCalled_ShouldNotHaveErrors_For_ValidShortUrlRequest()
    {
        //Arrange
        ShortUrlRequest shortUrlRequest = new("https://dotnet.microsoft.com/en-us/download/dotnet/8.0");
        ShortUrlRequestValidator validator = new();

        //Act
        var result = validator.Validate(shortUrlRequest);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenCalled_ShouldHaveErrors_For_InvalidShortUrlRequest()
    {
        //Arrange
        ShortUrlRequest shortUrlRequest = new("");
        ShortUrlRequestValidator validator = new();

        //Act
        var result = validator.Validate(shortUrlRequest);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LongUrl");
    }
}
