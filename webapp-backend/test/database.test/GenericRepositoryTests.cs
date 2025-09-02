
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using dataaccess.Repositories;
using dataaccess;
using model;

namespace dataaccess.tests;

public class GenericRepositoryTests
{
    private (ApplicationDbContext ctx, GenericRepository<TaskItem> repo) CreateRepo(string name)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        var ctx = new ApplicationDbContext(options);
        var repo = new GenericRepository<TaskItem>(ctx);
        return (ctx, repo);
    }

    private static TaskItem NewItem(string title, bool done = false) =>
        new TaskItem { Title = title, Description = "d", IsCompleted = done, CreatedAt = DateTime.UtcNow };

    [Fact]
    public async Task Add_Update_Delete_Works()
    {
        var (ctx, repo) = CreateRepo(nameof(Add_Update_Delete_Works));

        var item = NewItem("A");
        await repo.AddAsync(item);
        await ctx.SaveChangesAsync();

        var fetched = await repo.GetByIdAsync(item.Id);
        Assert.NotNull(fetched);
        Assert.Equal("A", fetched!.Title);

        fetched.Title = "B";
        repo.Update(fetched);
        await ctx.SaveChangesAsync();

        var updated = await repo.GetByIdAsync(item.Id);
        Assert.Equal("B", updated!.Title);

        repo.Delete(updated!);
        await ctx.SaveChangesAsync();

        var deleted = await repo.GetByIdAsync(item.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task GetAllAsync_Filter_OrderBy_Works()
    {
        var (ctx, repo) = CreateRepo(nameof(GetAllAsync_Filter_OrderBy_Works));

        await repo.AddAsync(NewItem("C"));
        await repo.AddAsync(NewItem("A"));
        await repo.AddAsync(NewItem("B", done: true));
        await ctx.SaveChangesAsync();

        var onlyIncomplete = await repo.GetAllAsync(filter: t => !t.IsCompleted);
        Assert.All(onlyIncomplete, t => Assert.False(t.IsCompleted));

        var ordered = await repo.GetAllAsync(orderBy: q => q.OrderBy(t => t.Title));
        Assert.Equal(new[] {"A","B","C"}, ordered.Select(i => i.Title).OrderBy(x => x).ToArray());
    }

    [Fact]
    public async Task AnyAsync_Works()
    {
        var (ctx, repo) = CreateRepo(nameof(AnyAsync_Works));
        await repo.AddAsync(NewItem("X"));
        await ctx.SaveChangesAsync();

        var exists = await repo.AnyAsync(t => t.Title == "X");
        var notExists = await repo.AnyAsync(t => t.Title == "Nope");
        Assert.True(exists);
        Assert.False(notExists);
    }
}
