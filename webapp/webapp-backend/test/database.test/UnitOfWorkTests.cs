
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using dataaccess;
using IUOF = dataaccess.UnitOfWork.IUnitOfWork;
using UOF = dataaccess.UnitOfWork.UnitOfWork;
using model;

namespace dataaccess.tests;

public class UnitOfWorkTests
{
    private (ApplicationDbContext ctx, IUOF uow) CreateUow(string name)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        var ctx = new ApplicationDbContext(options);
        var uow = new UOF(ctx);
        return (ctx, uow);
    }

    [Fact]
    public async Task SaveChangesAsync_Persists_Entities()
    {
        var (ctx, uow) = CreateUow(nameof(SaveChangesAsync_Persists_Entities));

        await uow.TaskItems.AddAsync(new TaskItem { Title = "T1" });
        var saved = await uow.SaveChangesAsync();

        Assert.Equal(1, saved);
        var all = await uow.TaskItems.GetAllAsync();
        Assert.Single(all);
    }

    [Fact]
    public async Task DisposeAsync_Disposes_Context()
    {
        var (ctx, uow) = CreateUow(nameof(DisposeAsync_Disposes_Context));
        await uow.DisposeAsync();
        // After dispose, accessing context should throw/object state invalid.
        await Assert.ThrowsAnyAsync<System.ObjectDisposedException>(async () =>
        {
            await ctx.Database.EnsureCreatedAsync();
        });
    }

    [Fact]
    public void Dispose_Disposes_Context()
    {
        var (ctx, uow) = CreateUow(nameof(Dispose_Disposes_Context));
        uow.Dispose();
        Assert.ThrowsAny<System.ObjectDisposedException>(() => ctx.Database.EnsureCreated());
    }
}
