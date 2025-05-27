using System.Text.Json;

using VictorFrye.CrudCounter.WebApi;

using Resource = VictorFrye.CrudCounter.WebApi.Resource;

namespace VictorFrye.CrudCounter.Integration.Tests.WebApi;

public class GetResourceByIdTests : TestAppHost
{
    [Fact]
    public async Task GetFoodResourceByIdReturnsOkResult()
    {
        var foodId = WellKnownResources.Food.Id;

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.GetAsync($"/api/resources/{foodId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<Resource>(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(foodId, result.Id);
    }

    [Fact]
    public async Task GetResourceByUnknownIdReturnsNotFoundResult()
    {
        var unknownId = Guid.NewGuid();

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.GetAsync($"/api/resources/{unknownId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var result = JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(unknownId.ToString(), result);
    }
}
