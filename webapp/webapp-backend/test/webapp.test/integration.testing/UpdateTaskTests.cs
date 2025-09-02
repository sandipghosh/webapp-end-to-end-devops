using System.Net.Http.Json;
using Xunit;

namespace webapp.test.integration.testing;

public class UpdateTaskTests: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public readonly string endpointPath = "/api/tasks";

    public UpdateTaskTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanUpdateTask()
    {
        var created = await _client.PostAsJsonAsync(endpointPath, new { Title = "Old", Description = "Old" });
        var task = await created.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = task["id"].ToString();

        var updateResponse = await _client.PutAsJsonAsync($"{endpointPath}/{id}", new { Title = "New", Description = "Updated" });
        updateResponse.EnsureSuccessStatusCode();

        var updated = await updateResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.Equal("New", updated["title"].ToString());
    }
}
