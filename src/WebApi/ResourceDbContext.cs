using Microsoft.EntityFrameworkCore;

namespace VictorFrye.CrudCounter.WebApi;

public class ResourceDbContext(DbContextOptions<ResourceDbContext> options) : DbContext(options)
{
    public DbSet<Resource> Resources => Set<Resource>();
}
