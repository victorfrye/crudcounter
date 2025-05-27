using System.Net.Http.Json;
using System.Text.Json;

using Resource = VictorFrye.CrudCounter.WebApi.Resource;

namespace VictorFrye.CrudCounter.Integration.Tests.WebApi;

public class PutResourceByIdTests : TestAppHost
{
    [Fact]
    public async Task PutWaterResourceByIdReturnsOkResult()
    {
        var water = new Resource()
        {
            Id = Guid.NewGuid(),
            Name = "Water",
            Count = 100,
        };

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.PutAsJsonAsync($"/api/resources/{water.Id}", water, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<Resource>(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(water, result);
    }
}
