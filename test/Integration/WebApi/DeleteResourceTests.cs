using System.Text.Json;

using VictorFrye.CrudCounter.WebApi;

namespace VictorFrye.CrudCounter.Integration.Tests.WebApi;

public class DeleteResourceByIdTests : TestAppHost
{
    [Fact]
    public async Task DeleteStoneResourceByIdReturnsOkResult()
    {
        var stoneId = WellKnownResources.Stone.Id;

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.DeleteAsync($"/api/resources/{stoneId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteResourceByUnknownIdReturnsNotFoundResult()
    {
        var unknownId = Guid.NewGuid();

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.DeleteAsync($"/api/resources/{unknownId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var result = JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(unknownId.ToString(), result);
    }
}
