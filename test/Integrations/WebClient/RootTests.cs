namespace VictorFrye.Counter.Testing.Integrations.WebClient;

public class RootTests : TestAppHost
{
    [Fact]
    public async Task GetResourceRootReturnsOkStatusCode()
    {
        var http = App.CreateHttpClient(Resources.WebClient);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebClient, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
