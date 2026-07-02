using TodoApp.Domain.Entities;
using TodoApp.Application.Responses;

namespace TodoApp.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<Response<UserEntity?>> GetByIdAsync(Guid id);

    Task<Response<UserEntity?>> GetByEmailAsync(string email);

    Task<Response> AddAsync(UserEntity user);

    Task<Response<bool>> ExistsByEmailAsync(string email);

    Task<Response> SaveChangesAsync();
}
