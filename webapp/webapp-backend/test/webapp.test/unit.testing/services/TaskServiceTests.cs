using Xunit;
using Microsoft.EntityFrameworkCore;
using webapp.Services;
using webapp.Models;
using Moq;
using dataaccess.UnitOfWork;
using dataaccess.Repositories;
using model;
using System.Linq;
using System.Linq.Expressions;


namespace webapp.test.unit.testing.services;

public class TaskServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IGenericRepository<TaskItem>> _mockRepo;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<IGenericRepository<TaskItem>>();

        // Assuming IUnitOfWork has a Tasks property of type IGenericRepository<TaskItem>
        _mockUow.Setup(u => u.TaskItems).Returns(_mockRepo.Object);

        _service = new TaskService(_mockUow.Object);
    }

    [Fact]
    public async Task GetAllTasks_ReturnsTasks()
    {
        var expected = new List<TaskItem> { new TaskItem { Id = Guid.NewGuid(), Title = "Task1" } };
        _mockRepo.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<TaskItem, bool>>?>(),
            It.IsAny<Func<IQueryable<TaskItem>, IOrderedQueryable<TaskItem>>?>(),
            It.IsAny<string>()))
        .ReturnsAsync(expected);

        var result = await _service.GetAll();

        Assert.Single(result);
        Assert.Equal("Task1", result.FirstOrDefault()?.Title);
    }

    [Fact]
    public async Task GetTaskById_ReturnsTask()
    {
        var id = Guid.NewGuid();
        var task = new TaskItem { Id = Guid.NewGuid(), Title = "FindMe" };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(task);

        var result = await _service.Get(id);

        Assert.NotNull(result);
        Assert.Equal("FindMe", result!.Title);
    }

    [Fact]
    public async Task CreateTask_AddsTask_AndCommits()
    {
        // Arrange
        var ct = CancellationToken.None;
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            Description = "Test description"
        };

        TaskItem? capturedEntity = null;

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
                .Callback<TaskItem>(t => capturedEntity = t)
                .Returns(Task.CompletedTask);

        _mockUow.Setup(u => u.TaskItems).Returns(_mockRepo.Object);
        _mockUow.Setup(u => u.SaveChangesAsync(ct)).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(request, ct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Task", result.Title);
        Assert.Equal("Test description", result.Description);
        Assert.NotEqual(default, result.CreatedAt); // CreatedAt is set

        _mockRepo.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }


    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenEntityFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ct = CancellationToken.None;
        var entity = new TaskItem { Id = id, Title = "Old", Description = "Old Desc", IsCompleted = false };
        var request = new UpdateTaskRequest { Title = "New", Description = "New Desc", IsCompleted = true };

        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _mockUow.Setup(u => u.TaskItems).Returns(_mockRepo.Object);
        _mockUow.Setup(u => u.SaveChangesAsync(ct)).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(id, request, ct);

        // Assert
        Assert.True(result);
        Assert.Equal("New", entity.Title);
        Assert.Equal("New Desc", entity.Description);
        Assert.True(entity.IsCompleted);

        _mockRepo.Verify(r => r.Update(entity), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenEntityNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Simulate "not found" from repository
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TaskItem?)null);
        _mockUow.Setup(u => u.TaskItems).Returns(_mockRepo.Object);

        // Act
        var result = await _service.DeleteAsync(id, ct);

        // Assert
        Assert.False(result);

        _mockRepo.Verify(r => r.Delete(It.IsAny<TaskItem>()), Times.Never);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

}
