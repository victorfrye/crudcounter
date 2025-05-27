using Aspire.Hosting;

namespace VictorFrye.CrudCounter.Integration.Tests;

public abstract class TestAppHost : IAsyncLifetime
{
    public DistributedApplication App = null!;

    public ResourceNotificationService ResourceNotificationService => App.Services.GetRequiredService<ResourceNotificationService>();

    public static class Resources
    {
        public const string WebApi = "api";
        public const string WebClient = "client";
    }

    public async ValueTask InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();

        App = await appHost.BuildAsync();
        await App.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await App.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
