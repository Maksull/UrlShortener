using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDataContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("UrlShortenerDb"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapUrlEndpoints();

app.Run();
