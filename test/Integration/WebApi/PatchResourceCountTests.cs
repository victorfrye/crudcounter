using System.Text.Json;

using VictorFrye.CrudCounter.WebApi;
using Resource = VictorFrye.CrudCounter.WebApi.Resource;

namespace VictorFrye.CrudCounter.Integration.Tests.WebApi;

public class PatchResourceCountTests : TestAppHost
{
    [Fact]
    public async Task PatchGoldResourceByIdReturnsOkResult()
    {
        var goldId = WellKnownResources.Gold.Id;
        const int expectedCount = 1000;

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.PatchAsync($"/api/resources/{goldId}/count/{expectedCount}", null, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<Resource>(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(goldId, result.Id);
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public async Task PatchResourceByUnknownIdReturnsNotFoundResult()
    {
        var unknownId = Guid.NewGuid();
        const int expectedCount = 100;

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.PatchAsync($"/api/resources/{unknownId}/count/{expectedCount}", null, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var result = JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(unknownId.ToString(), result);
    }
}
