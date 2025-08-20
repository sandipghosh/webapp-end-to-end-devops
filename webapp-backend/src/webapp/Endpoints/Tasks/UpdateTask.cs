using FastEndpoints;
using webapp.Models;
using webapp.Services;

namespace webapp.Endpoints.Tasks;

public class UpdateTaskEndpoint : Endpoint<UpdateTaskRequest, EmptyResponse>
{
    private readonly TaskService _service;

    public UpdateTaskEndpoint(TaskService service) => _service = service;

    public override void Configure()
    {
        Put("/api/tasks/{id}");
        AllowAnonymous();
        Description(x => x.WithName("UpdateTask"));
    }

    public override async Task HandleAsync(UpdateTaskRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var success = await _service.UpdateAsync(id, req, ct);

        if (!success)
            await Send.NotFoundAsync(ct);

        await Send.NoContentAsync(ct);
    }
}
