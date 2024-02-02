using HashidsNet;
using Infrastructure.Data;
using Infrastructure.Mediatr.Handlers.Urls;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDataContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("UrlShortenerDb")).EnableSensitiveDataLogging();
});


builder.Services.AddSingleton<IHashids>(_ => new Hashids("UrlShortener"));


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ShortUrlHandler>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapUrlEndpoints();

app.Run();
