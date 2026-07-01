using Microsoft.EntityFrameworkCore;
using TodoApp.Application.DTOs.Pagination;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Domain.Entities;
using TodoApp.Persistence.Context;

namespace TodoApp.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<TaskItem>> GetPagedAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search,
        Guid? categoryId)
    {
        IQueryable<TaskItem> query = _context.Tasks
            .Include(t => t.Category)
            .Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Title.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == categoryId);
        }

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PagedResult<TaskItem>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}