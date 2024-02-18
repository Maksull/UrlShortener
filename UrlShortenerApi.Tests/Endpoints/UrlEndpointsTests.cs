using MediatR;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerApi.Endpoints;

namespace UrlShortenerApi.Tests.Endpoints;

public sealed class UrlEndpointsTests
{
    private readonly IMediator _mediator;

    public UrlEndpointsTests()
    {
        _mediator = Substitute.For<IMediator>();
    }

    #region CreateCategory

    [Fact]
    public async void ShortUrl_WhenCalled_ReturnOk()
    {
        //Arrange
        /*var category = _categoryFaker.Generate();
        CreateCategoryRequest createCategory = new()
        {
            Name = category.Name
        };
        _mediator.Send(Arg.Any<CreateCategoryCommand>())
            .ReturnsForAnyArgs(category);

        //Act
        var t = await UrlEndpoints.Sh
        var response = (await _controller.CreateCategory(createCategory, CancellationToken.None) as OkObjectResult)!;
        var result = response.Value as CategoryResponse;

        //Assert
        response.Should().BeOfType<OkObjectResult>();
        result.Should().BeOfType<CategoryResponse>();
        result.Should().BeEquivalentTo(category, opts =>
            opts.Excluding(c => c.CategoryId)
        );*/
    }

    #endregion
}
