using Microsoft.EntityFrameworkCore;

namespace VictorFrye.Counter.WebApi;

public class ResourceDbContext(DbContextOptions<ResourceDbContext> options) : DbContext(options)
{
    public DbSet<Resource> Resources => Set<Resource>();
}
