using Microsoft.Extensions.DependencyInjection;

namespace VictorFrye.CrudCounter.Core;

public static class ResourceServicesExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddResourceServices() =>
             services.AddScoped<IResourceRepository, ResourceRepository>()
                     .AddScoped<IResourceService, ResourceService>();
    }
}
