using model;
using webapp.Models;

namespace webapp.Services;

public interface ITaskService
{
    Task<TaskItem> CreateAsync(CreateTaskRequest req, CancellationToken ct);
    Task<TaskItem?> Get(Guid id);
    Task<IEnumerable<TaskItem>> GetAll();
    Task<bool> UpdateAsync(Guid id, UpdateTaskRequest req, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
