using FastEndpoints;
using model;
using webapp.Models;
using webapp.Services;

namespace webapp.Endpoints.Tasks;

public class UpdateTaskEndpoint : Endpoint<UpdateTaskRequest, TaskItem>
{
    private readonly ITaskService _service;

    public UpdateTaskEndpoint(ITaskService service) => _service = service;

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

        var refreshed = await _service.Get(id);
        await Send.OkAsync(refreshed, ct);
    }
}
