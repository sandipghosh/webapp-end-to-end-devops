using FastEndpoints;
using webapp.Services;

namespace webapp.Endpoints.Tasks;

public class DeleteTaskEndpoint : EndpointWithoutRequest
{
    private readonly TaskService _service;

    public DeleteTaskEndpoint(TaskService service) => _service = service;

    public override void Configure()
    {
        Delete("/api/tasks/{id}");
        AllowAnonymous();
        Description(x => x.WithName("DeleteTask"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var success = await _service.DeleteAsync(id, ct);

        if (!success)
            await Send.NotFoundAsync(ct);

        await Send.NoContentAsync(ct);
    }
}

