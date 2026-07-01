using TodoApp.Application.DTOs.Categories;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color
        };
    }

    public async Task UpdateAsync(Guid id, CreateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        // Check duplicate name for the same user (excluding current category)
        var existing = await _categoryRepository.GetByNameAsync(category.UserId, dto.Name);
        if (existing is not null && existing.Id != id)
        {
            throw new InvalidOperationException("Category with the same name already exists.");
        }

        category.Name = dto.Name;
        category.Color = dto.Color;

        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId)
    {
        var categories = await _categoryRepository.GetAllByUserAsync(userId);

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Color = c.Color
        });
    }

    public async Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto dto)
    {
        // Check duplicate name for the same user
        var existing = await _categoryRepository.GetByNameAsync(userId, dto.Name);
        if (existing is not null)
        {
            throw new InvalidOperationException("Category with the same name already exists.");
        }

        var category = new Category
        {
            Name = dto.Name,
            Color = dto.Color,
            UserId = userId
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        await _categoryRepository.DeleteAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }
}
