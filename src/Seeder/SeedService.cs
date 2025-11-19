using System.Diagnostics;

using Microsoft.EntityFrameworkCore;

using VictorFrye.CrudCounter.Core;

namespace VictorFrye.CrudCounter.Seeder;

public class SeedService(
    IServiceProvider provider,
    IHostApplicationLifetime lifetime,
    ILogger<SeedService> logger) : BackgroundService
{
    public const string ActivitySourceName = "Seeder";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Seeding database", ActivityKind.Client);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Starting database seeding...");
        }

        try
        {
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ResourceDbContext>();

            await ExecuteMigrations(context, stoppingToken);
            await SeedData(context, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Database seeding complete. Stopping seeder.");
        }

        lifetime.StopApplication();
    }

    private async Task ExecuteMigrations(ResourceDbContext context, CancellationToken stoppingToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Executing database migrations...");
        }

        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () => await context.Database.MigrateAsync(stoppingToken));

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Database migrated.");
        }
    }

    private async Task SeedData(ResourceDbContext context, CancellationToken stoppingToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Seeding database with well-known resources...");
        }

        foreach (var resource in WellKnownResources.StartingResources)
        {
            var entity = await context.Resources.FirstOrDefaultAsync(r => r.Id == resource.Id, cancellationToken: stoppingToken);

            if (entity is null)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Seeding resource {ResourceId} - {ResourceName}", resource.Id, resource.Name);
                }

                context.Add(resource);
            }
        }

        await context.SaveChangesAsync(stoppingToken);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Database seed complete.");
        }
    }
}
