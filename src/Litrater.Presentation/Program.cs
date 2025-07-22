using Asp.Versioning;
using HealthChecks.UI.Client;
using Litrater.Application;
using Litrater.Infrastructure;
using Litrater.Presentation;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddApplication()
    .AddPresentation(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();

var versionedGroup = app.MapGroup("api/v{apiVersion:apiVersion}")
    .WithApiVersionSet(apiVersionSet)
    .WithOpenApi();

app.MapEndpoints(versionedGroup);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.Services.MigrateDatabaseAsync();
    await app.Services.SeedDatabaseAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseUserSync();
app.UseRequestContextLogging();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

await app.RunAsync();

// Make the Program class available for testing
public abstract partial class Program
{
}