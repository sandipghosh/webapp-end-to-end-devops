
using webapp.Models;
using model;
using dataaccess.UnitOfWork;

namespace webapp.Services;

public class TaskService
{
    private readonly IUnitOfWork _uow;
    public TaskService(IUnitOfWork uow) => _uow = uow;

    //private readonly List<TaskItem> _tasks = new();

    public async Task<IEnumerable<TaskItem>> GetAll() => await _uow.TaskItems.GetAllAsync();

    public async Task<TaskItem?> Get(Guid id) => await _uow.TaskItems.GetByIdAsync(id);
    //_tasks.FirstOrDefault(t => t.Id == id);

    public async Task<TaskItem> CreateAsync(CreateTaskRequest req, CancellationToken ct)
    {
        var entity = new TaskItem
        {
            Title = req.Title,
            Description = req.Description,
            CreatedAt = DateTime.UtcNow
        };
        //_tasks.Add(task);

        await _uow.TaskItems.AddAsync(entity);
        await _uow.SaveChangesAsync(ct);

        return await Task.FromResult(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTaskRequest req, CancellationToken ct)
    {
        var entity = await Get(id);
        if (entity is null)
            return await Task.FromResult(false);

        entity.Title = req.Title;
        entity.Description = req.Description;
        entity.IsCompleted = req.IsCompleted;

        _uow.TaskItems.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await Get(id);
        if (entity is null) 
            return await Task.FromResult(false);

        _uow.TaskItems.Delete(entity);
        await _uow.SaveChangesAsync(ct);
        return await Task.FromResult(true);
    }
}
