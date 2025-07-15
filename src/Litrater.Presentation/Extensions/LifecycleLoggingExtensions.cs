namespace Litrater.Presentation.Extensions;

public static class LifecycleLoggingExtensions
{
    public static IServiceCollection AddLifecycleLogging(this IServiceCollection services)
    {
        services.AddHostedService<LifecycleLoggingService>();
        return services;
    }
}

internal class LifecycleLoggingService(ILogger<LifecycleLoggingService> logger, IHostApplicationLifetime lifetime) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        lifetime.ApplicationStarted.Register(() => logger.LogInformation("Litrater application has started"));
        lifetime.ApplicationStopping.Register(() => logger.LogInformation("Litrater application is stopping"));
        lifetime.ApplicationStopped.Register(() => logger.LogInformation("Litrater application has stopped"));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}