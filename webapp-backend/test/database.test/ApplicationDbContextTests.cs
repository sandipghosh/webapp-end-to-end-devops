using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using dataaccess;
using model;

namespace dataaccess.tests;

public class ApplicationDbContextTests
{
    private ApplicationDbContext CreateSqliteInMemoryContext()
    {
        // Create a new in-memory SQLite connection
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open(); // Keep the connection open for the lifetime of the context

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated(); // Ensure schema is created
        return context;
    }

    [Fact]
    public void Model_Has_TaskItems_DbSet()
    {
        using var ctx = CreateSqliteInMemoryContext();
        var set = ctx.TaskItems;
        Assert.NotNull(set);
    }

    [Fact]
    public void OnModelCreating_TaskItem_Title_IsRequired_And_MaxLength200()
    {
        using var ctx = CreateSqliteInMemoryContext();
        var entity = ctx.Model.FindEntityType(typeof(TaskItem));
        var title = entity!.FindProperty(nameof(TaskItem.Title))!;
        Assert.True(!title.IsNullable);
        Assert.Equal(200, title.GetMaxLength());
    }

    [Fact]
    public void OnModelCreating_TaskItem_IsCompleted_Default_False()
    {
        using var ctx = CreateSqliteInMemoryContext();
        var entity = ctx.Model.FindEntityType(typeof(TaskItem));
        var prop = entity!.FindProperty(nameof(TaskItem.IsCompleted))!;
        Assert.Equal(false, prop.GetDefaultValue());
    }

    [Fact]
    public void OnModelCreating_TaskItem_CreatedAt_HasColumnType_Timestamp()
    {
        using var ctx = CreateSqliteInMemoryContext();
        var entity = ctx.Model.FindEntityType(typeof(TaskItem));
        var prop = entity!.FindProperty(nameof(TaskItem.CreatedAt))!;
        Assert.Equal("timestamp", prop.GetColumnType());
    }
}
