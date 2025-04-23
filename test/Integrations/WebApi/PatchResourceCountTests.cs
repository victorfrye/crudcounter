using System.Net.Http.Json;
using System.Text.Json;

using VictorFrye.Counter.WebApi;

using Resource = VictorFrye.Counter.WebApi.Resource;

namespace VictorFrye.Counter.Testing.Integrations.WebApi;

public class PatchResourceCountTests : TestAppHost
{
    [Fact]
    public async Task PatchGoldResourceByIdReturnsOkResult()
    {
        var goldId = WellKnownResources.Gold.Id;
        const int expectedCount = 1000;

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.PatchAsync($"/api/resources/{goldId}/count/{expectedCount}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<Resource>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(goldId, result.Id);
        Assert.Equal(expectedCount, result.Count);
    }
}
