using System;
using dataaccess.Repositories;
using model;

namespace dataaccess.UnitOfWork;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IGenericRepository<TaskItem> TaskItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
