using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using Resource = VictorFrye.Counter.WebApi.Resource;

namespace VictorFrye.Counter.Testing.Integrations.WebApi;

public class PostResourceTests : TestAppHost
{
    [Fact]
    public async Task PostElectricityResourceReturnsCreatedResult()
    {
        var electricity = new Resource
        {
            Id = Guid.NewGuid(),
            Name = "Electricity",
            Count = 200,
        };

        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.PostAsJsonAsync("/api/resources", electricity);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/api/resources/{electricity.Id}", response.Headers.Location.ToString());

        var result = JsonSerializer.Deserialize<Resource>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
        Assert.Equal(electricity.Id, result.Id);
    }

    [Fact]
    public async Task PostNullResourceReturnsInternalServerErrorResult()
    {
        var http = App.CreateHttpClient(Resources.WebApi);
        await ResourceNotificationService.WaitForResourceAsync(Resources.WebApi, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await http.PostAsJsonAsync("/api/resources", null as Resource);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var result = JsonSerializer.Deserialize<ProblemDetails>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
        Assert.NotNull(result);
    }
}
