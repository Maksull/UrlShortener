using Core.Validators.Urls;
using FluentValidation;
using FluentValidation.AspNetCore;
using HashidsNet;
using Infrastructure.BackgroundServices.Interfaces;
using Infrastructure.BackgroundServices;
using Infrastructure.Data;
using Infrastructure.Mediatr.Behaviors;
using Infrastructure.Mediatr.Handlers.Urls;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Caching;
using UrlShortenerApi.Middlewares;
using Hangfire;
using Hangfire.PostgreSql;

namespace UrlShortenerApi.Dependencies;

public static class Dependencies
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureDataContext(configuration);
        services.ConfigureExceptionHandler();
        services.ConfigureRedisOutputCache(configuration);
        services.ConfigureFluentValidation();
        services.ConfigureHashIds();
        services.ConfigureMediatR();
        services.ConfigureHangfire(configuration);

        return services;
    }

    private static void ConfigureDataContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApiDataContext>(opts =>
        {
            opts.UseNpgsql(configuration.GetConnectionString("UrlShortenerDb")).EnableSensitiveDataLogging();
        });
    }

    private static void ConfigureExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    private static void ConfigureRedisOutputCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = configuration.GetConnectionString("RedisCache");
        });

        services.AddRedisOutputCache(opts =>
        {
            opts.AddBasePolicy(builder =>
                builder.Expire(TimeSpan.FromSeconds(30)));
        });
    }

    private static void ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ShortUrlRequestValidator).Assembly);
        services.AddFluentValidationAutoValidation();
    }

    private static void ConfigureMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ShortUrlHandler>());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }

    private static void ConfigureHashIds(this IServiceCollection services)
    {
        services.AddSingleton<IHashids>(_ => new Hashids("UrlShortener"));
    }

    private static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(configuration.GetConnectionString("HangfireConnection"))
            )
        );
        services.AddHangfireServer();

        services.AddScoped<IDeleteService, DeleteService>();

    }
}
