using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VictorFrye.SimpleCrud.ApiService;

public class ResourceHealthCheckPublisher : IHealthCheckPublisher
{
    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken = default)
    {
        if (HealthStatus.Healthy == report.Status)
        {
            Console.WriteLine("All resources are bountiful!");
            return Task.CompletedTask;
        }

        foreach (var entry in report.Entries)
        {
            if (HealthStatus.Healthy == entry.Value.Status)
            {
                continue;
            }

            Console.WriteLine($"We are low on {entry.Key}... Please dedicate villagers to gather more.");

            // Publish the health check result to a message queue or other service?
        }

        return Task.CompletedTask;
    }
}
