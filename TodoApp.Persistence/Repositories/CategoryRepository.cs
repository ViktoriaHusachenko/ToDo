using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Domain.Entities;
using TodoApp.Persistence.Context;

namespace TodoApp.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllByUserAsync(Guid userId)
    {
        return await _context.Categories
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetByNameAsync(Guid userId, string name)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == name);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    public Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}