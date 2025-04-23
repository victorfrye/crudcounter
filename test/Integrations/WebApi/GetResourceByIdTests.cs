using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using VictorFrye.Counter.WebApi;

using Resource = VictorFrye.Counter.WebApi.Resource;

namespace VictorFrye.Counter.Testing.Integrations.WebApi;

public class GetResourceByIdTests : TestAppHost
{
    [Fact]
    public async Task GetFoodResourceByIdReturnsOkResult()
    {
        var foodId = WellKnownResources.Food.Id;

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.GetAsync($"/api/resources/{foodId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<Resource>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(foodId, result.Id);
    }

    [Fact]
    public async Task GetResourceByUnknownIdReturnsNotFoundResult()
    {
        var unknownId = Guid.NewGuid();

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.GetAsync($"/api/resources/{unknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var result = JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(unknownId.ToString(), result);
    }
}
