using System.Text.Json;

using Resource = VictorFrye.CrudCounter.WebApi.Resource;

namespace VictorFrye.CrudCounter.Integration.Tests.WebApi;

public class GetResourcesTests : TestAppHost
{
    [Fact]
    public async Task GetResourcesReturnsOkResult()
    {
        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.GetAsync("/api/resources", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonSerializer.Deserialize<IEnumerable<Resource>>(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
