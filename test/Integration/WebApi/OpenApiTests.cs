using Microsoft.OpenApi.Models;


namespace VictorFrye.CrudCounter.Integration.Tests.WebApi;

public class OpenApiTests : TestAppHost
{
    [Fact]
    public async Task GetOpenApiReturnsJsonDocument()
    {
        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);

        var result = await OpenApiDocument.LoadAsync(stream, null, null, TestContext.Current.CancellationToken);

        Assert.NotNull(result.Document);
        Assert.Equal("Resources API", result.Document.Info.Title);
        Assert.Equal("v1", result.Document.Info.Version);

        Assert.Equal(3, result.Document.Paths.Count);

        Assert.Contains(result.Document.Paths, p => p.Key == "/api/resources");
        var resourcesPath = result.Document.Paths["/api/resources"];
        Assert.NotNull(resourcesPath.Operations);
        Assert.Equal(2, resourcesPath.Operations.Count);
        Assert.Contains(resourcesPath.Operations, o => o.Key == HttpMethod.Get);
        Assert.Contains(resourcesPath.Operations, o => o.Key == HttpMethod.Post);

        Assert.Contains(result.Document.Paths, p => p.Key == "/api/resources/{id}");
        var resourceByIdPath = result.Document.Paths["/api/resources/{id}"];
        Assert.NotNull(resourceByIdPath.Operations);
        Assert.Equal(3, resourceByIdPath.Operations.Count);
        Assert.Contains(resourceByIdPath.Operations, o => o.Key == HttpMethod.Get);
        Assert.Contains(resourceByIdPath.Operations, o => o.Key == HttpMethod.Put);
        Assert.Contains(resourceByIdPath.Operations, o => o.Key == HttpMethod.Delete);

        Assert.Contains(result.Document.Paths, p => p.Key == "/api/resources/{id}/count/{count}");
        var resourceCountPath = result.Document.Paths["/api/resources/{id}/count/{count}"];
        Assert.NotNull(resourceCountPath.Operations);
        Assert.Single(resourceCountPath.Operations);
        Assert.Contains(resourceCountPath.Operations, o => o.Key == HttpMethod.Patch);
    }
}
