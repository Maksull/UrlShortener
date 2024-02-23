using Core.Entities;
using Core.Mediatr.Queries;
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
        _client = _factory.CreateClient();
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
}
