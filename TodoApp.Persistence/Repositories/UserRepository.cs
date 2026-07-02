using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Application.Responses;
using TodoApp.Domain.Entities;
using TodoApp.Persistence.Context;

namespace TodoApp.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<Response<UserEntity?>> GetByIdAsync(Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        return Response<UserEntity?>.Ok(user);
    }

    public async Task<Response<UserEntity?>> GetByEmailAsync(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);
        return Response<UserEntity?>.Ok(user);
    }

    public async Task<Response<bool>> ExistsByEmailAsync(string email)
    {
        var exists = await context.Users.AnyAsync(x => x.Email == email);
        return Response<bool>.Ok(exists);
    }

    public async Task<Response> AddAsync(UserEntity user)
    {
        await context.Users.AddAsync(user);
        return Response.Ok();
    }

    public async Task<Response> SaveChangesAsync()
    {
        await context.SaveChangesAsync();
        return Response.Ok();
    }
}