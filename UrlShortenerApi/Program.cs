using Hangfire;
using UrlShortenerApi.Dependencies;
using UrlShortenerApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("UrlShortenerFrontend");

app.UseOutputCache();

app.UseHangfireDashboard();

app.AddHangfireJobs();

app.UseExceptionHandler();

app.MapUrlEndpoints();

app.Run();
