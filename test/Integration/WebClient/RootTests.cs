namespace VictorFrye.CrudCounter.Integration.Tests.WebClient;

public class RootTests : TestAppHost
{
    [Fact]
    public async Task GetResourceRootReturnsOkStatusCode()
    {
        var http = App.CreateHttpClient(Resources.WebClient);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebClient, KnownResourceStates.Running, TestContext.Current.CancellationToken)
                                         .WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken);
        var response = await http.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
