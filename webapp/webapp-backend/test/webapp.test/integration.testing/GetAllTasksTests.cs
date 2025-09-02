using System.Net.Http.Json;
using Xunit;

namespace webapp.test.integration.testing;

public class GetAllTasksTests: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public readonly string endpointPath = "/api/tasks";

    public GetAllTasksTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ReturnsEmptyList_WhenNoTasksExist()
    {
        var response = await _client.GetAsync(endpointPath);
        response.EnsureSuccessStatusCode();

        var tasks = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

}
