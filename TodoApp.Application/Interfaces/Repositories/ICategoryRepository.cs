using TodoApp.Domain.Entities;
using TodoApp.Application.Responses;

namespace TodoApp.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Response<IEnumerable<CategoryEntity>>> GetAllByUserAsync(Guid userId);

    Task<Response<CategoryEntity?>> GetByIdAsync(Guid id);

    Task<Response<CategoryEntity?>> GetByNameAsync(Guid userId, string name);

    Task<Response> AddAsync(CategoryEntity category);

    Task<Response> UpdateAsync(CategoryEntity category);

    Task<Response> DeleteAsync(CategoryEntity category);

    Task<Response> SaveChangesAsync();
}
