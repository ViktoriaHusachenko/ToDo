using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);

    Task AddAsync(User user);

    Task<bool> ExistsByEmailAsync(string email);

    Task SaveChangesAsync();
}