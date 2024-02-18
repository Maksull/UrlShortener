using Core.Mediatr.Commands;
using Core.Validators.Urls;

namespace Core.Tests.Validators;

public sealed class ShortUrlCommandValidatorTests
{
    [Fact]
    public void Validate_WhenCalled_ShouldNotHaveErrors_For_ValidShortUrlCommand()
    {
        //Arrange
        ShortUrlCommand shortUrlRequest = new("https://localhost:7167", "https://dotnet.microsoft.com/en-us/download/dotnet/8.0");
        ShortUrlCommandValidator validator = new();

        //Act
        var result = validator.Validate(shortUrlRequest);

        //Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenCalled_ShouldHaveErrors_For_InvalidShortUrlCommand()
    {
        //Arrange
        ShortUrlCommand shortUrlRequest = new("", "");
        ShortUrlCommandValidator validator = new();

        //Act
        var result = validator.Validate(shortUrlRequest);

        //Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AppUrl");
        result.Errors.Should().Contain(e => e.PropertyName == "LongUrl");
    }
}
