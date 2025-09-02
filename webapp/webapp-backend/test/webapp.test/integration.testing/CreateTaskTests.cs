using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace webapp.test.integration.testing;

public class CreateTaskTests: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public readonly string endpointPath = "/api/tasks";

    public CreateTaskTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanCreateTask_AndRetrieveIt()
    {
        var request = new { Title = "Integration Task", Description = "Test" };

        var response = await _client.PostAsJsonAsync(endpointPath, request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(created);
        Assert.Equal("Integration Task", created["title"].ToString());

        var id = created["id"].ToString();
        var getResponse = await _client.GetFromJsonAsync<Dictionary<string, object>>($"{endpointPath}/{id}");
        Assert.Equal("Integration Task", getResponse["title"].ToString());
    }

}
