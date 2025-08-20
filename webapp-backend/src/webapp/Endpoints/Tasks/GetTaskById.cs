using FastEndpoints;
using model;
using webapp.Services;

namespace webapp.Endpoints.Tasks;
public class GetTaskByIdEndpoint : Endpoint<GetTaskByIdRequest, TaskItem>
{
    private readonly TaskService _service;

    public GetTaskByIdEndpoint(TaskService service) => _service = service;

    public override void Configure()
    {
        Get("/api/tasks/{id}");
        AllowAnonymous();
        Description(x => x.WithName("GetTaskById"));
    }

    public override async Task HandleAsync(GetTaskByIdRequest req, CancellationToken ct)
    {
        var task = await _service.Get(req.Id);
        if (task is null)
            await Send.NotFoundAsync(ct);
        else
            await Send.OkAsync(task, ct);
    }
}

public class GetTaskByIdRequest
{
    public Guid Id { get; set; }
}
