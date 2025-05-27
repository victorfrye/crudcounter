using Microsoft.EntityFrameworkCore;

namespace VictorFrye.CrudCounter.WebApi;

public static class SeedExtensions
{
    public static void SeedMockData(this DbContext context)
    {
        foreach (var resource in WellKnownResources.StartingResources)
        {
            var res = context.Set<Resource>().FirstOrDefault(r => r.Id == resource.Id);

            if (res is null)
            {
                context.Add(resource);

                context.SaveChanges();
            }
        }
    }

    public static async Task SeedMockDataAsync(this DbContext context, CancellationToken cancellationToken = default)
    {
        foreach (var resource in WellKnownResources.StartingResources)
        {
            var res = await context.Set<Resource>().FirstOrDefaultAsync(r => r.Id == resource.Id, cancellationToken: cancellationToken);
            if (res is null)
            {
                await context.AddAsync(resource, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
