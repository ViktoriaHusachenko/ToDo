using Microsoft.EntityFrameworkCore;
using TodoApp.Application.DTOs.Pagination;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Application.Responses;
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

    public async Task<Response<PagedResult<TaskItemEntity>>> GetPagedAsync(
        Guid userId,
        PaginationParameters pagination,
        string? search,
        Guid? categoryId)
    {
        IQueryable<TaskItemEntity> query = _context.Tasks
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

        var result = new PagedResult<TaskItemEntity>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        return Response<PagedResult<TaskItemEntity>>.Ok(result);
    }

    public async Task<Response<TaskItemEntity?>> GetByIdAsync(Guid id)
    {
        var task = await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

        return Response<TaskItemEntity?>.Ok(task);
    }

    public async Task<Response> AddAsync(TaskItemEntity task)
    {
        await _context.Tasks.AddAsync(task);
        return Response.Ok();
    }

    public Task<Response> UpdateAsync(TaskItemEntity task)
    {
        _context.Tasks.Update(task);
        return Task.FromResult(Response.Ok());
    }

    public Task<Response> DeleteAsync(TaskItemEntity task)
    {
        _context.Tasks.Remove(task);
        return Task.FromResult(Response.Ok());
    }

    public async Task<Response> SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
        return Response.Ok();
    }
}