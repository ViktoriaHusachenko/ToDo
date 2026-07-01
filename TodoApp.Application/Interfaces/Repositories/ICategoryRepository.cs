using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllByUserAsync(Guid userId);

    Task<Category?> GetByIdAsync(Guid id);

    Task<Category?> GetByNameAsync(Guid userId, string name);

    Task AddAsync(Category category);

    Task UpdateAsync(Category category);

    Task DeleteAsync(Category category);

    Task SaveChangesAsync();
}