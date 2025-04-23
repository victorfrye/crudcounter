using System.Text.Json;

using Resource = VictorFrye.Counter.WebApi.Resource;

namespace VictorFrye.Counter.Testing.Integrations.WebApi;

public class GetResourcesTests : TestAppHost
{
    [Fact]
    public async Task GetResourcesReturnsOkResult()
    {
        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.GetAsync("/api/resources");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<IEnumerable<Resource>>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
