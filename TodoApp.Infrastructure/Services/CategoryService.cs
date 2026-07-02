using System.Net;
using System.Linq;
using TodoApp.Application.DTOs.Categories;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Application.Responses;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Response<CategoryDto?>> GetByIdAsync(Guid id)
    {
        var categoryResp = await _categoryRepository.GetByIdAsync(id);
        if (categoryResp.IsError) return Response<CategoryDto?>.Error(categoryResp.ErrorMessage ?? "An error occurred.");

        var category = categoryResp.Value;
        if (category is null) return Response<CategoryDto?>.Ok(null, HttpStatusCode.NotFound);

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color
        };

        return Response<CategoryDto?>.Ok(dto);
    }

    public async Task<Response> UpdateAsync(Guid id, CreateCategoryDto dto)
    {
        var categoryResp = await _categoryRepository.GetByIdAsync(id);
        if (categoryResp.IsError) return Response.Error(categoryResp.ErrorMessage ?? "An error occurred.");

        var category = categoryResp.Value;
        if (category is null) return Response.Error("Category not found.", HttpStatusCode.NotFound);

        // Check duplicate name for the same user (excluding current category)
        var existingResp = await _categoryRepository.GetByNameAsync(category.UserId, dto.Name);
        if (existingResp.IsError) return Response.Error(existingResp.ErrorMessage ?? "An error occurred.");

        var existing = existingResp.Value;
        if (existing is not null && existing.Id != id)
        {
            return Response.Error("Category with the same name already exists.", HttpStatusCode.BadRequest);
        }

        category.Name = dto.Name;
        category.Color = dto.Color;

        var updateResp = await _categoryRepository.UpdateAsync(category);
        if (updateResp.IsError) return Response.Error(updateResp.ErrorMessage ?? "Failed to update category.");

        var saveResp = await _categoryRepository.SaveChangesAsync();
        if (saveResp.IsError) return Response.Error(saveResp.ErrorMessage ?? "Failed to save changes.");

        return Response.Ok();
    }

    public async Task<Response<IEnumerable<CategoryDto>>> GetAllAsync(Guid userId)
    {
        var categoriesResp = await _categoryRepository.GetAllByUserAsync(userId);
        if (categoriesResp.IsError) return Response<IEnumerable<CategoryDto>>.Error(categoriesResp.ErrorMessage ?? "An error occurred.");

        var categories = categoriesResp.Value;

        var items = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Color = c.Color
        });

        return Response<IEnumerable<CategoryDto>>.Ok(items);
    }

    public async Task<Response<CategoryDto>> CreateAsync(Guid userId, CreateCategoryDto dto)
    {
        var existingResp = await _categoryRepository.GetByNameAsync(userId, dto.Name);
        if (existingResp.IsError) return Response<CategoryDto>.Error(existingResp.ErrorMessage ?? "An error occurred.");
        if (existingResp.Value is not null) return Response<CategoryDto>.Error("Category with the same name already exists.", HttpStatusCode.BadRequest);

        var category = new CategoryEntity
        {
            Name = dto.Name,
            Color = dto.Color,
            UserId = userId
        };

        var addResp = await _categoryRepository.AddAsync(category);
        if (addResp.IsError) return Response<CategoryDto>.Error(addResp.ErrorMessage ?? "Failed to add category.");

        var saveResp = await _categoryRepository.SaveChangesAsync();
        if (saveResp.IsError) return Response<CategoryDto>.Error(saveResp.ErrorMessage ?? "Failed to save changes.");

        var dtoResult = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color
        };

        return Response<CategoryDto>.Ok(dtoResult, HttpStatusCode.Created);
    }

    public async Task<Response> DeleteAsync(Guid id)
    {
        var categoryResp = await _categoryRepository.GetByIdAsync(id);
        if (categoryResp.IsError) return Response.Error(categoryResp.ErrorMessage ?? "An error occurred.");

        var category = categoryResp.Value;
        if (category is null) return Response.Error("Category not found.", HttpStatusCode.NotFound);

        var delResp = await _categoryRepository.DeleteAsync(category);
        if (delResp.IsError) return Response.Error(delResp.ErrorMessage ?? "Failed to delete category.");

        var saveResp = await _categoryRepository.SaveChangesAsync();
        if (saveResp.IsError) return Response.Error(saveResp.ErrorMessage ?? "Failed to save changes.");

        return Response.Ok();
    }
}
