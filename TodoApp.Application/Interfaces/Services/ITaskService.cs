using TodoApp.Application.DTOs.Pagination;
using TodoApp.Application.DTOs.Tasks;

namespace TodoApp.Application.Interfaces.Services;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> GetPagedAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search,
        Guid? categoryId);

    Task<TaskDto?> GetByIdAsync(Guid id);

    Task<TaskDto> CreateAsync(
        Guid userId,
        CreateTaskDto dto);

    Task UpdateAsync(
        Guid userId,
        UpdateTaskDto dto);

    Task DeleteAsync(Guid id);

    Task CompleteAsync(Guid id);

    Task UncompleteAsync(Guid id);
}