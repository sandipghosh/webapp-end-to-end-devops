using FastEndpoints;
using model;
using webapp.Models;
using webapp.Services;

namespace webapp.Endpoints.Tasks;

public class CreateTaskEndpoint : Endpoint<CreateTaskRequest, TaskItem>
{
    private readonly ITaskService _service;

    public CreateTaskEndpoint(ITaskService service) => _service = service;

    public override void Configure()
    {
        Post("/api/tasks");
        AllowAnonymous();
        Description(x => x.WithName("CreateTask"));
    }

    public override async Task HandleAsync(CreateTaskRequest req, CancellationToken ct)
    {
        var task = await _service.CreateAsync(req, ct);
        await Send.CreatedAtAsync<GetTaskByIdEndpoint>(new { id = task.Id }, task);
    }
}
