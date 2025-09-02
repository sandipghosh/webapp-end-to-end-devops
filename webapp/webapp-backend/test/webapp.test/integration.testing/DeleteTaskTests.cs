using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace webapp.test.integration.testing;

public class DeleteTaskTests: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public readonly string endpointPath = "/api/tasks";

    public DeleteTaskTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanDeleteTask()
    {
        var created = await _client.PostAsJsonAsync(endpointPath, new { Title = "ToDelete", Description = "Temp" });
        var task = await created.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = task["id"].ToString();

        var deleteResponse = await _client.DeleteAsync($"{endpointPath}/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"{endpointPath}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

}
