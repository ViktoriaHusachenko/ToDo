using TodoApp.Application.DTOs.Pagination;
using TodoApp.Application.DTOs.Tasks;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<PagedResult<TaskDto>> GetPagedAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search,
        Guid? categoryId)
    {
        var paged = await _taskRepository.GetPagedAsync(userId, pagination, search, categoryId);

        var items = paged.Items.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            DueDate = t.DueDate,
            IsCompleted = t.IsCompleted,
            Priority = t.Priority,
            CategoryName = t.Category?.Name
        });

        return new PagedResult<TaskDto>
        {
            Items = items,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null) return null;

        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            Priority = task.Priority,
            CategoryName = task.Category?.Name
        };
    }

    public async Task<TaskDto> CreateAsync(Guid userId, CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            IsCompleted = false,
            Priority = dto.Priority,
            CategoryId = dto.CategoryId,
            UserId = userId
        };

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            Priority = task.Priority,
            CategoryName = task.Category?.Name
        };
    }

    public async Task UpdateAsync(Guid userId, UpdateTaskDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(dto.Id);
        if (task is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        if (task.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to modify this task.");
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.DueDate = dto.DueDate;
        task.IsCompleted = dto.IsCompleted;
        task.Priority = dto.Priority;
        task.CategoryId = dto.CategoryId;

        await _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        await _taskRepository.DeleteAsync(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task CompleteAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null) throw new KeyNotFoundException("Task not found.");

        task.IsCompleted = true;
        await _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task UncompleteAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null) throw new KeyNotFoundException("Task not found.");

        task.IsCompleted = false;
        await _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();
    }
}
