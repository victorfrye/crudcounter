using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VictorFrye.SimpleCrud.ApiService;

public abstract class ResourceHealthCheck(ResourceDbContext dbContext) : IHealthCheck
{
    public abstract Resource Resource { get; }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var res = await dbContext.Resources.FindAsync([Resource.Id], cancellationToken);

        if (res == null)
        {
            return HealthCheckResult.Unhealthy($"Resource {Resource.Name} does not exist! Doooooom.");
        }

        IReadOnlyDictionary<string, object> data = new Dictionary<string, object>()
        {
            { Resource.Name, res }
        };

        return res.Count > 99
            ? HealthCheckResult.Healthy($"Our {Resource.Name} stockpiles are high!", data: data)
            : new HealthCheckResult(context.Registration.FailureStatus, $"Our {Resource.Name} stockpiles are low!", data: data);
    }
}

public class FoodHealthCheck(ResourceDbContext dbContext) : ResourceHealthCheck(dbContext)
{
    public override Resource Resource => WellKnownResources.Food;
}

public class WoodHealthCheck(ResourceDbContext dbContext) : ResourceHealthCheck(dbContext)
{
    public override Resource Resource => WellKnownResources.Wood;
}

public class GoldHealthCheck(ResourceDbContext dbContext) : ResourceHealthCheck(dbContext)
{
    public override Resource Resource => WellKnownResources.Gold;
}

public class StoneHealthCheck(ResourceDbContext dbContext) : ResourceHealthCheck(dbContext)
{
    public override Resource Resource => WellKnownResources.Stone;
}
