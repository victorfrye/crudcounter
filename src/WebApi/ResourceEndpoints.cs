using Microsoft.EntityFrameworkCore;

namespace VictorFrye.CrudCounter.WebApi;

public static class ResourceEndpoints
{
    internal const string BaseRoute = "/api/resources";

    public static IEndpointRouteBuilder MapResourceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(BaseRoute, PostResource);
        endpoints.MapGet(BaseRoute, GetResources);
        endpoints.MapGet($"{BaseRoute}/{{id:guid}}", GetResourceById);
        endpoints.MapPut($"{BaseRoute}/{{id:guid}}", PutResource);
        endpoints.MapPatch($"{BaseRoute}/{{id:guid}}/count/{{count:int}}", PatchResourceCount);
        endpoints.MapDelete($"{BaseRoute}/{{id:guid}}", DeleteResource);

        return endpoints;
    }

    public static async Task<IResult> PostResource(Resource resource, ResourceDbContext db, CancellationToken cancellationToken)
    {
        db.Resources.Add(resource);
        await db.SaveChangesAsync(cancellationToken);

        return Results.Created($"{BaseRoute}/{resource.Id}", resource);
    }

    public static async Task<IResult> GetResources(ResourceDbContext db, CancellationToken cancellationToken)
    {
        var resources = await db.Resources.ToListAsync(cancellationToken);
        return Results.Ok(resources);
    }

    public static async Task<IResult> GetResourceById(Guid id, ResourceDbContext db, CancellationToken cancellationToken)
    {
        var resource = await db.Resources.FindAsync([id], cancellationToken);
        return resource is null ? Results.NotFound(id) : Results.Ok(resource);
    }

    public static async Task<IResult> PutResource(Guid id, Resource resource, ResourceDbContext db, CancellationToken cancellationToken)
    {
        var existingResource = await db.Resources.FindAsync([id], cancellationToken);

        if (existingResource is null)
        {
            existingResource = new Resource
            {
                Id = id,
                Name = resource.Name,
                Count = resource.Count
            };
            db.Resources.Add(existingResource);
        }
        else
        {
            existingResource.Name = resource.Name;
            existingResource.Count = resource.Count;
            db.Resources.Update(existingResource);
        }

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(existingResource);
    }

    public static async Task<IResult> PatchResourceCount(Guid id, int count, ResourceDbContext db, CancellationToken cancellationToken)
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
    }

    public static async Task<IResult> DeleteResource(Guid id, ResourceDbContext db, CancellationToken cancellationToken)
    {
        var resource = await db.Resources.FindAsync([id], cancellationToken);

        if (resource is null)
        {
            return Results.NotFound(id);
        }

        db.Resources.Remove(resource);
        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
