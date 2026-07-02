using TodoApp.Application.DTOs.Pagination;
using TodoApp.Application.DTOs.Tasks;
using TodoApp.Application.Responses;

namespace TodoApp.Application.Interfaces.Services;

public interface ITaskService
{
    Task<Response<PagedResult<TaskDto>>> GetPagedAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search,
        Guid? categoryId);

    Task<Response<TaskDto>> GetByIdAsync(Guid id);

    Task<Response<TaskDto>> CreateAsync(Guid userId, CreateTaskDto dto);

    Task<Response> UpdateAsync(Guid userId, UpdateTaskDto dto);

    Task<Response> DeleteAsync(Guid id);

    Task<Response> CompleteAsync(Guid id);

    Task<Response> UncompleteAsync(Guid id);
}
