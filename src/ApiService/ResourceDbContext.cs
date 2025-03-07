using Microsoft.EntityFrameworkCore;

namespace VictorFrye.SimpleCrud.ApiService;

public class ResourceDbContext(DbContextOptions<ResourceDbContext> options) : DbContext(options)
{
    public DbSet<Resource> Resources => Set<Resource>();
}
