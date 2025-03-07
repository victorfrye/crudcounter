using System.Collections.Frozen;

using Microsoft.EntityFrameworkCore;

namespace VictorFrye.SimpleCrud.ApiService;

public static class ResourceDbSeed
{
    private const string Food = nameof(Food);
    private const string Wood = nameof(Wood);
    private const string Gold = nameof(Gold);
    private const string Stone = nameof(Stone);

    private static readonly Guid FoodGuid = Guid.Parse("0195725a-eb24-7f14-8b2f-d131671684f1");
    private static readonly Guid WoodGuid = Guid.Parse("0195725b-5031-72cc-8969-16dd38932381");
    private static readonly Guid GoldGuid = Guid.Parse("0195725b-b77e-7ed0-9387-9dca428f3337");
    private static readonly Guid StoneGuid = Guid.Parse("0195725b-ed2f-7842-8305-ceb37b249472");

    private static readonly FrozenSet<Resource> startingResources = [
            new()
            {
                Id = FoodGuid,
                Name = Food,
                Count = 200
            },
            new()
            {
                Id = WoodGuid,
                Name = Wood,
                Count = 200
            },
             new()
            {
                Id = GoldGuid,
                Name = Gold,
                Count = 100
            },
            new()
            {
                Id = StoneGuid,
                Name = Stone,
                Count = 100
            }
        ];

    public static void SeedMockData(this DbContext context)
    {
        foreach (var resource in startingResources)
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
        foreach (var resource in startingResources)
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
