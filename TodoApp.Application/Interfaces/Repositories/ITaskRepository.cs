using TodoApp.Application.DTOs.Pagination;
using TodoApp.Domain.Entities;
using TodoApp.Application.Responses;

namespace TodoApp.Application.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<Response<PagedResult<TaskItemEntity>>> GetPagedAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search,
        Guid? categoryId);

    Task<Response<TaskItemEntity>> GetByIdAsync(Guid id);

    Task<Response> AddAsync(TaskItemEntity task);

    Task<Response> UpdateAsync(TaskItemEntity task);

    Task<Response> DeleteAsync(TaskItemEntity task);

    Task<Response> SaveChangesAsync();
}
