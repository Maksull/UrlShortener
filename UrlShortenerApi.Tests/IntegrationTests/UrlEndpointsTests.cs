using Core.Contracts.UrlEndpoints;
using Core.Entities;
using HashidsNet;
using Infrastructure.Data;
using Meziantou.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace UrlShortenerApi.Tests.IntegrationTests;

[DisableParallelization]
public sealed class UrlEndpointsTests
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IHashids _hashids;

    public UrlEndpointsTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b =>
            {
                b.ConfigureServices(services =>
                {
                    var descriptor =
                        services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApiDataContext>));

                    if (descriptor is not null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApiDataContext>(opts =>
                    {
                        opts.UseInMemoryDatabase("InMemoryDatabaseUrls");
                    });
                });
            });
        var opts = new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        };
        _client = _factory.CreateClient(opts);
        _hashids = new Hashids("UrlShortener");
    }

    [Fact]
    public async Task GetOriginalUrl_WhenUrl_ReturnOk()
    {
        //Arrange
        long urlId = 1;
        var code = _hashids.EncodeLong(urlId);
        var originalUrl = "https://example.com";

        Url url = new()
        {
            Id = urlId,
            OriginalUrl = originalUrl,
            Code = code,
            ShortenedUrl = $"https://localhost:7167/{code}",
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddMinutes(5)
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApiDataContext>();

            await dbContext.Database.EnsureCreatedAsync();
            dbContext.Urls.RemoveRange(dbContext.Urls);
            await dbContext.Urls.AddAsync(url);
            await dbContext.SaveChangesAsync();
        }

        //Act
        var response = await _client.GetAsync($"{url.Code}+");
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<string>(responseContent);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeEquivalentTo(originalUrl);
    }

    [Fact]
    public async Task GetOriginalUrl_WhenNoUrl_ReturnNotFound()
    {
        //Arrange
        long urlId = 1;
        var code = _hashids.EncodeLong(urlId);
        var originalUrl = "https://example.com";

        Url url = new()
        {
            Id = urlId,
            OriginalUrl = originalUrl,
            Code = code,
            ShortenedUrl = $"https://localhost:7167/{code}",
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddMinutes(5)
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApiDataContext>();

            await dbContext.Database.EnsureCreatedAsync();
            dbContext.Urls.RemoveRange(dbContext.Urls);
            await dbContext.SaveChangesAsync();
        }

        //Act
        var response = await _client.GetAsync($"{url.Code}+");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RedirectToOriginalUrl_WhenUrl_ReturnRedirect()
    {
        //Arrange
        long urlId = 1;
        var code = _hashids.EncodeLong(urlId);
        var originalUrl = "https://example.com/";

        Url url = new()
        {
            Id = urlId,
            OriginalUrl = originalUrl,
            Code = code,
            ShortenedUrl = $"https://localhost:7167/{code}",
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddMinutes(5)
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApiDataContext>();

            await dbContext.Database.EnsureCreatedAsync();
            dbContext.Urls.RemoveRange(dbContext.Urls);
            await dbContext.Urls.AddAsync(url);
            await dbContext.SaveChangesAsync();
        }

        //Act
        var response = await _client.GetAsync($"{url.Code}");
        var destination = response.Headers.Location?.ToString();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        destination.Should().BeEquivalentTo(originalUrl);
    }

    [Fact]
    public async Task RedirectToOriginalUrl_WhenUrl_ReturnNotFound()
    {
        //Arrange
        long urlId = 1;
        var code = _hashids.EncodeLong(urlId);
        var originalUrl = "https://example.com";

        Url url = new()
        {
            Id = urlId,
            OriginalUrl = originalUrl,
            Code = code,
            ShortenedUrl = $"https://localhost:7167/{code}",
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddMinutes(5)
        };

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApiDataContext>();

            await dbContext.Database.EnsureCreatedAsync();
            dbContext.Urls.RemoveRange(dbContext.Urls);
            await dbContext.SaveChangesAsync();
        }

        //Act
        var response = await _client.GetAsync($"{url.Code}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShortUrl_WhenCalled_ReturnOk()
    {
        // Arrange
        var originalUrl = "https://example.com";

        ShortUrlRequest shortUrlRequest = new(originalUrl);

        var content = new StringContent(JsonSerializer.Serialize(shortUrlRequest),
            System.Text.Encoding.UTF8,
            "application/json");

        // Act
        var postResponse = await _client.PostAsync("", content);
        postResponse.EnsureSuccessStatusCode();
        var shortenedUrl =
            JsonSerializer.Deserialize<string>(await postResponse.Content.ReadAsStringAsync())!;
        string code = shortenedUrl.Substring(shortenedUrl.LastIndexOf('/') + 1);

        var getResponse = await _client.GetAsync($"{code}+");
        getResponse.EnsureSuccessStatusCode();

        var responseContent = await getResponse.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<string>(responseContent)!;

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        shortenedUrl.Should().BeOfType<string>();
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeOfType<string>();
    }
}
