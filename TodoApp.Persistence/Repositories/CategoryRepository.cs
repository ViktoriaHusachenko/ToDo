using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Application.Responses;
using TodoApp.Domain.Entities;
using TodoApp.Persistence.Context;

namespace TodoApp.Persistence.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<Response<IEnumerable<CategoryEntity>>> GetAllByUserAsync(Guid userId)
    {
        var list = await context.Categories
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Response<IEnumerable<CategoryEntity>>.Ok(list);
    }

    public async Task<Response<CategoryEntity?>> GetByIdAsync(Guid id)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        return Response<CategoryEntity?>.Ok(category);
    }

    public async Task<Response<CategoryEntity?>> GetByNameAsync(Guid userId, string name)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == name);

        return Response<CategoryEntity?>.Ok(category);
    }

    public async Task<Response> AddAsync(CategoryEntity category)
    {
        await context.Categories.AddAsync(category);
        return Response.Ok();
    }

    public Task<Response> UpdateAsync(CategoryEntity category)
    {
        context.Categories.Update(category);
        return Task.FromResult(Response.Ok());
    }

    public Task<Response> DeleteAsync(CategoryEntity category)
    {
        context.Categories.Remove(category);
        return Task.FromResult(Response.Ok());
    }

    public async Task<Response> SaveChangesAsync()
    {
        await context.SaveChangesAsync();
        return Response.Ok();
    }
}