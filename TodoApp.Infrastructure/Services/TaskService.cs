using System.Net;
using System.Linq;
using TodoApp.Application.DTOs.Pagination;
using TodoApp.Application.DTOs.Tasks;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Application.Responses;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Services;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public async Task<Response<PagedResult<TaskDto>>> GetPagedAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search,
        Guid? categoryId)
    {
        var pagedResp = await taskRepository.GetPagedAsync(userId, pagination, search, categoryId);
        if (pagedResp.IsError) return Response<PagedResult<TaskDto>>.Error(pagedResp.ErrorMessage ?? "An error occurred.");

        var paged = pagedResp.Value;

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

        var result = new PagedResult<TaskDto>
        {
            Items = items,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };

        return Response<PagedResult<TaskDto>>.Ok(result);
    }

    public async Task<Response<TaskDto?>> GetByIdAsync(Guid id)
    {
        var resp = await taskRepository.GetByIdAsync(id);
        if (resp.IsError) return Response<TaskDto?>.Error(resp.ErrorMessage ?? "An error occurred.");

        var task = resp.Value;
        if (task is null) return Response<TaskDto?>.Ok(null, HttpStatusCode.NotFound);

        var dto = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            Priority = task.Priority,
            CategoryName = task.Category?.Name
        };

        return Response<TaskDto?>.Ok(dto);
    }

    public async Task<Response<TaskDto>> CreateAsync(Guid userId, CreateTaskDto dto)
    {
        var task = new TaskItemEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            IsCompleted = false,
            Priority = dto.Priority,
            CategoryId = dto.CategoryId,
            UserId = userId
        };

        var addResp = await taskRepository.AddAsync(task);
        if (addResp.IsError) return Response<TaskDto>.Error(addResp.ErrorMessage ?? "Failed to add task.");

        var saveResp = await taskRepository.SaveChangesAsync();
        if (saveResp.IsError) return Response<TaskDto>.Error(saveResp.ErrorMessage ?? "Failed to save task.");

        var dtoResult = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            Priority = task.Priority,
            CategoryName = task.Category?.Name
        };

        return Response<TaskDto>.Ok(dtoResult, HttpStatusCode.Created);
    }

    public async Task<Response> UpdateAsync(Guid userId, UpdateTaskDto dto)
    {
        var resp = await taskRepository.GetByIdAsync(dto.Id);
        if (resp.IsError) return Response.Error(resp.ErrorMessage ?? "An error occurred.");

        var task = resp.Value;
        if (task is null) return Response.Error("Task not found.", HttpStatusCode.NotFound);

        if (task.UserId != userId) return Response.Error("You are not allowed to modify this task.", HttpStatusCode.Forbidden);

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.DueDate = dto.DueDate;
        task.IsCompleted = dto.IsCompleted;
        task.Priority = dto.Priority;
        task.CategoryId = dto.CategoryId;

        var updateResp = await taskRepository.UpdateAsync(task);
        if (updateResp.IsError) return Response.Error(updateResp.ErrorMessage ?? "Failed to update task.");

        var saveResp = await taskRepository.SaveChangesAsync();
        if (saveResp.IsError) return Response.Error(saveResp.ErrorMessage ?? "Failed to save changes.");

        return Response.Ok();
    }

    public async Task<Response> DeleteAsync(Guid id)
    {
        var resp = await taskRepository.GetByIdAsync(id);
        if (resp.IsError) return Response.Error(resp.ErrorMessage ?? "An error occurred.");

        var task = resp.Value;
        if (task is null) return Response.Error("Task not found.", HttpStatusCode.NotFound);

        var delResp = await taskRepository.DeleteAsync(task);
        if (delResp.IsError) return Response.Error(delResp.ErrorMessage ?? "Failed to delete task.");

        var saveResp = await taskRepository.SaveChangesAsync();
        if (saveResp.IsError) return Response.Error(saveResp.ErrorMessage ?? "Failed to save changes.");

        return Response.Ok();
    }

    public async Task<Response> CompleteAsync(Guid id)
    {
        var resp = await taskRepository.GetByIdAsync(id);
        if (resp.IsError) return Response.Error(resp.ErrorMessage ?? "An error occurred.");

        var task = resp.Value;
        if (task is null) return Response.Error("Task not found.", HttpStatusCode.NotFound);

        task.IsCompleted = true;

        var updateResp = await taskRepository.UpdateAsync(task);
        if (updateResp.IsError) return Response.Error(updateResp.ErrorMessage ?? "Failed to update task.");

        var saveResp = await taskRepository.SaveChangesAsync();
        if (saveResp.IsError) return Response.Error(saveResp.ErrorMessage ?? "Failed to save changes.");

        return Response.Ok();
    }

    public async Task<Response> UncompleteAsync(Guid id)
    {
        var resp = await taskRepository.GetByIdAsync(id);
        if (resp.IsError) return Response.Error(resp.ErrorMessage ?? "An error occurred.");

        var task = resp.Value;
        if (task is null) return Response.Error("Task not found.", HttpStatusCode.NotFound);

        task.IsCompleted = false;

        var updateResp = await taskRepository.UpdateAsync(task);
        if (updateResp.IsError) return Response.Error(updateResp.ErrorMessage ?? "Failed to update task.");

        var saveResp = await taskRepository.SaveChangesAsync();
        if (saveResp.IsError) return Response.Error(saveResp.ErrorMessage ?? "Failed to save changes.");

        return Response.Ok();
    }

}
