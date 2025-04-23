using Microsoft.EntityFrameworkCore;

namespace VictorFrye.Counter.WebApi;

public static class ResourceEndpoints
{
    public const string ResourceApiBaseRoute = "/api/resources";

    public static IEndpointRouteBuilder MapResourceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // MARK: CREATE
        endpoints.MapPost(ResourceApiBaseRoute, async (Resource resource, ResourceDbContext db, CancellationToken cancellationToken) =>
        {
            db.Resources.Add(resource);
            await db.SaveChangesAsync(cancellationToken);

            return Results.Created($"{ResourceApiBaseRoute}/{resource.Id}", resource);
        });

        // MARK: READ
        endpoints.MapGet(ResourceApiBaseRoute, async (ResourceDbContext db, CancellationToken cancellationToken) =>
        {
            var resources = await db.Resources.ToListAsync(cancellationToken);

            return Results.Ok(resources);
        })
        .CacheOutput(policy => policy.Expire(TimeSpan.FromSeconds(5)));

        endpoints.MapGet($"{ResourceApiBaseRoute}/{{id:guid}}", async (Guid id, ResourceDbContext db, CancellationToken cancellationToken) =>
        {
            var resource = await db.Resources.FindAsync([id], cancellationToken);

            return resource is null ? Results.NotFound(id) : Results.Ok(resource);
        })
        .CacheOutput(policy => policy.Expire(TimeSpan.FromSeconds(5)));

        // MARK: UPDATE
        endpoints.MapPut($"{ResourceApiBaseRoute}/{{id:guid}}", async (Guid id, Resource resource, ResourceDbContext db, CancellationToken cancellationToken) =>
        {
            var existingResource = await db.Resources.FindAsync([id], cancellationToken);

            if (existingResource is null)
            {
                db.Resources.Add(new Resource
                {
                    Id = id,
                    Name = resource.Name,
                    Count = resource.Count
                });
            }
            else
            {
                existingResource.Name = resource.Name;
                existingResource.Count = resource.Count;
                db.Resources.Update(existingResource);
            }

            await db.SaveChangesAsync(cancellationToken);

            return Results.Ok(existingResource);
        });

        endpoints.MapPatch($"{ResourceApiBaseRoute}/{{id:guid}}/count/{{count:int}}", async (Guid id, int count, ResourceDbContext db, CancellationToken cancellationToken) =>
        {
            var resource = await db.Resources.FindAsync([id], cancellationToken);

            if (resource is null)
            {
                return Results.NotFound(id);
            }

            resource.Count = count;
            db.Resources.Update(resource);

            await db.SaveChangesAsync(cancellationToken);

            return Results.Ok(resource);
        });

        // MARK: DELETE
        endpoints.MapDelete($"{ResourceApiBaseRoute}/{{id:guid}}", async (Guid id, ResourceDbContext db, CancellationToken cancellationToken) =>
        {
            var resource = await db.Resources.FindAsync([id], cancellationToken);

            if (resource is null)
            {
                return Results.NotFound(id);
            }

            db.Resources.Remove(resource);
            await db.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });

        return endpoints;
    }
}
