using Core.Validators.Urls;
using FluentValidation;
using FluentValidation.AspNetCore;
using HashidsNet;
using Infrastructure.Data;
using Infrastructure.Mediatr.Behaviors;
using Infrastructure.Mediatr.Handlers.Urls;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Endpoints;
using UrlShortenerApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache(opts =>
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


var app = builder.Build();

app.UseOutputCache();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapUrlEndpoints();

app.Run();
