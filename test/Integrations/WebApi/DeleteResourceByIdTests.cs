using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using VictorFrye.Counter.WebApi;

using Resource = VictorFrye.Counter.WebApi.Resource;

namespace VictorFrye.Counter.Testing.Integrations.WebApi;

public class DeleteResourceByIdTests : TestAppHost
{
    [Fact]
    public async Task DeleteStoneResourceByIdReturnsOkResult()
    {
        var stoneId = WellKnownResources.Stone.Id;

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.DeleteAsync($"/api/resources/{stoneId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Null(response.Content);
    }

    [Fact]
    public async Task DeleteResourceByUnknownIdReturnsNotFoundResult()
    {
        var unknownId = Guid.NewGuid();

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.DeleteAsync($"/api/resources/{unknownId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var result = JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(unknownId.ToString(), result);
    }
}
