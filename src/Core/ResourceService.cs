using Microsoft.EntityFrameworkCore;

namespace VictorFrye.CrudCounter.Core;

public interface IResourceService
{
    ValueTask<Resource> CreateResource(Resource resource, CancellationToken cancellationToken = default);
    ValueTask<IEnumerable<Resource>> RetrieveResources(CancellationToken cancellationToken = default);
    ValueTask<Resource?> RetrieveResourceById(Guid id, CancellationToken cancellationToken = default);
    ValueTask<Resource?> UpdateResource(Guid id, Resource resource, CancellationToken cancellationToken = default);
    ValueTask<Resource?> UpdateResourceCount(Guid id, int count, CancellationToken cancellationToken = default);
    ValueTask<bool> DeleteResource(Guid id, CancellationToken cancellationToken = default);
}

public class ResourceService(IResourceRepository repository) : IResourceService
{
    public async ValueTask<Resource> CreateResource(Resource resource, CancellationToken cancellationToken = default)
    {
        await using var repo = repository;

        var entry = repo.AddResource(resource);

        return entry.Entity;
    }

    public async ValueTask<IEnumerable<Resource>> RetrieveResources(CancellationToken cancellationToken = default)
    {
        await using var repo = repository;

        return await repo.GetResources()
                         .AsNoTracking()
                         .ToHashSetAsync(cancellationToken);
    }

    public async ValueTask<Resource?> RetrieveResourceById(Guid id, CancellationToken cancellationToken = default)
    {
        await using var repo = repository;

        return await repo.FindResource(id, cancellationToken);
    }

    public async ValueTask<Resource?> UpdateResource(Guid id, Resource resource, CancellationToken cancellationToken = default)
    {
        await using var repo = repository;

        var existingResource = await repo.FindResource(id, cancellationToken);

        if (existingResource is null)
        {
            existingResource = new Resource
            {
                Id = id,
                Name = resource.Name,
                Count = resource.Count
            };
            repo.AddResource(existingResource);
        }
        else
        {
            existingResource.Name = resource.Name;
            existingResource.Count = resource.Count;
            repo.UpdateResource(existingResource);
        }

        return existingResource;
    }

    public async ValueTask<Resource?> UpdateResourceCount(Guid id, int count, CancellationToken cancellationToken = default)
    {
        await using var repo = repository;

        var resource = await repo.FindResource(id, cancellationToken);

        if (resource is null)
        {
            return null;
        }

        resource.Count = count;
        repo.UpdateResource(resource);

        return resource;
    }

    public async ValueTask<bool> DeleteResource(Guid id, CancellationToken cancellationToken = default)
    {
        await using var repo = repository;

        var resource = await repo.FindResource(id, cancellationToken);

        if (resource is null)
        {
            return false;
        }

        repo.RemoveResource(resource);

        return true;
    }
}
