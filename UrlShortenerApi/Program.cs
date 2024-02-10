using Core.Validators.Urls;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using HashidsNet;
using Infrastructure.BackgroundServices;
using Infrastructure.BackgroundServices.Interfaces;
using Infrastructure.Data;
using Infrastructure.Mediatr.Behaviors;
using Infrastructure.Mediatr.Handlers.Urls;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Caching;
using UrlShortenerApi.Endpoints;
using UrlShortenerApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRedisOutputCache(opts =>
{
    opts.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromSeconds(30)));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<ApiDataContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("UrlShortenerDb")).EnableSensitiveDataLogging();
});

builder.Services.AddValidatorsFromAssembly(typeof(ShortUrlRequestValidator).Assembly);

builder.Services.AddFluentValidationAutoValidation();


builder.Services.AddSingleton<IHashids>(_ => new Hashids("UrlShortener"));


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ShortUrlHandler>());

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddStackExchangeRedisCache(opts =>
{
    opts.Configuration = builder.Configuration.GetConnectionString("RedisCache");
});

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(c =>
            c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HangfireConnection"))
        )
);
builder.Services.AddHangfireServer();

builder.Services.AddScoped<IDeleteService, DeleteService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseOutputCache();

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<IDeleteService>(
    "delete-urls",
    s => s.DeleteOldUrls(),
    Cron.Minutely(),
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Utc,
    });


app.UseExceptionHandler();

app.MapUrlEndpoints();

app.Run();
