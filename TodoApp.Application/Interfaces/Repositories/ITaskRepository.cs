using TodoApp.Application.DTOs.Pagination;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<PagedResult<TaskItem>> GetPagedAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search,
        Guid? categoryId);

    Task<TaskItem?> GetByIdAsync(Guid id);

    Task AddAsync(TaskItem task);

    Task UpdateAsync(TaskItem task);

    Task DeleteAsync(TaskItem task);

    Task SaveChangesAsync();
}