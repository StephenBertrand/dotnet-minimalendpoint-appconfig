using Api.Hosting;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDemoAzureAppConfiguration();
builder.Services.AddOpenApi();
builder.Services.AddVerticalSliceEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.MapDefaultEndpoints();

if (AzureAppConfigurationHosting.HasAppConfigurationConnection(app.Configuration)
    && app.Services.GetService<IConfigurationRefresherProvider>() is not null)
{
    // Per-request refresh is a cheap cache check until the sentinel / feature-flag interval elapses.
    app.UseAzureAppConfiguration();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapVerticalSlices();

app.Run();

public partial class Program;
