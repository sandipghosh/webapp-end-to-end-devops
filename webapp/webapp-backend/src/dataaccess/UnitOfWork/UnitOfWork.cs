using dataaccess.Repositories;
using model;

namespace dataaccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        TaskItems = new GenericRepository<TaskItem>(_context);
    }

    public IGenericRepository<TaskItem> TaskItems { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async ValueTask DisposeAsync() => await _context.DisposeAsync();

    public void Dispose()
    {
        _context.Dispose();
    }
}
