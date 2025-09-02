using FastEndpoints;
using Microsoft.VisualBasic;
using model;
using webapp.Services;

namespace webapp.Endpoints.Tasks;

public class GetAllTasksEndpoint : EndpointWithoutRequest<IEnumerable<TaskItem>>
{
    private readonly ITaskService _service;

    public GetAllTasksEndpoint(ITaskService service) => _service = service;

    public override void Configure()
    {
        Get("/api/tasks");
        AllowAnonymous();
        Description(x => x.WithName("GetAllTask"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tasks = await _service.GetAll();
        await Send.OkAsync(tasks, ct);
    }
}
