using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VictorFrye.CrudCounter.Core;

public interface IResourceRepository : IAsyncDisposable
{
    IQueryable<Resource> GetResources();
    ValueTask<Resource?> FindResource(Guid id, CancellationToken cancellationToken = default);
    EntityEntry<Resource> AddResource(Resource resource);
    EntityEntry<Resource> UpdateResource(Resource resource);
    EntityEntry<Resource> RemoveResource(Resource resource);
    Task<int> SaveChanges(CancellationToken cancellationToken = default);
}

public class ResourceRepository(ResourceDbContext context) : IResourceRepository
{
    public IQueryable<Resource> GetResources()
    {
        return context.Resources.AsQueryable();
    }

    public async ValueTask<Resource?> FindResource(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Resources.FindAsync([id], cancellationToken);
    }

    public EntityEntry<Resource> AddResource(Resource resource)
    {
        return context.Resources.Add(resource);
    }

    public EntityEntry<Resource> UpdateResource(Resource resource)
    {
        return context.Resources.Update(resource);
    }

    public EntityEntry<Resource> RemoveResource(Resource resource)
    {
        return context.Resources.Remove(resource);
    }

    public async Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (context.ChangeTracker.HasChanges())
        {
            await SaveChanges();
        }

        await context.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
