using Aspire.Hosting;

namespace VictorFrye.Counter.Testing.Integrations;

public abstract class TestAppHost : IAsyncLifetime
{
    protected DistributedApplication App = null!;

    protected ResourceNotificationService ResourceNotificationService => App.Services.GetRequiredService<ResourceNotificationService>();

    public static class Resources
    {
        public const string WebApi = "api";
        public const string WebClient = "client";
    }

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();

        App = await appHost.BuildAsync();
        await App.StartAsync();
    }

    public async Task DisposeAsync() => await App.DisposeAsync();
}
