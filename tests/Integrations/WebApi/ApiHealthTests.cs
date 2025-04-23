namespace VictorFrye.Counter.Testing.Integrations.WebApi;

public class ApiHealthTests : TestAppHost
{
    [Fact]
    public async Task GetLivenessReturnsOkStatusCode()
    {
        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.GetAsync("/alive");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealthReturnsOkStatusCode()
    {
        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
