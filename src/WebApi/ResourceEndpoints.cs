using VictorFrye.CrudCounter.Core;

namespace VictorFrye.CrudCounter.WebApi;

public static class ResourceEndpoints
{
    internal const string BaseRoute = "/api/resources";

    public static IEndpointRouteBuilder MapResourceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(BaseRoute, PostResource);
        endpoints.MapGet(BaseRoute, GetResources).CacheOutput(p => p.Expire(TimeSpan.FromSeconds(10)));
        endpoints.MapGet($"{BaseRoute}/{{id:guid}}", GetResourceById).CacheOutput(p => p.Expire(TimeSpan.FromSeconds(10)));
        endpoints.MapPut($"{BaseRoute}/{{id:guid}}", PutResource);
        endpoints.MapPatch($"{BaseRoute}/{{id:guid}}/count/{{count:int}}", PatchResourceCount);
        endpoints.MapDelete($"{BaseRoute}/{{id:guid}}", DeleteResource);

        return endpoints;
    }

    public static async Task<IResult> PostResource(Resource resource, IResourceService service, CancellationToken cancellationToken)
    {
        await service.CreateResource(resource, cancellationToken);

        return Results.Created($"{BaseRoute}/{resource.Id}", resource);
    }

    public static async Task<IResult> GetResources(IResourceService service, CancellationToken cancellationToken)
    {
        var resources = await service.RetrieveResources(cancellationToken);

        return Results.Ok(resources);
    }

    public static async Task<IResult> GetResourceById(Guid id, IResourceService service, CancellationToken cancellationToken)
    {
        var resource = await service.RetrieveResourceById(id, cancellationToken);

        return resource is null ? Results.NotFound(id) : Results.Ok(resource);
    }

    public static async Task<IResult> PutResource(Guid id, Resource resource, IResourceService service, CancellationToken cancellationToken)
    {
        var updatedResource = await service.UpdateResource(id, resource, cancellationToken);

        return Results.Ok(updatedResource);
    }

    public static async Task<IResult> PatchResourceCount(Guid id, int count, IResourceService service, CancellationToken cancellationToken)
    {
        var updatedResource = await service.UpdateResourceCount(id, count, cancellationToken);

        return updatedResource is null ? Results.NotFound(id) : Results.Ok(updatedResource);
    }

    public static async Task<IResult> DeleteResource(Guid id, IResourceService service, CancellationToken cancellationToken)
    {
        var result = await service.DeleteResource(id, cancellationToken);

        return result ? Results.NoContent() : Results.NotFound(id);
    }
}
