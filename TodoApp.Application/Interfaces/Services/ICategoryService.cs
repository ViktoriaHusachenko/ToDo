using TodoApp.Application.DTOs.Categories;

namespace TodoApp.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId);
    Task<CategoryDto?> GetByIdAsync(Guid id);

    Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto dto);

    Task UpdateAsync(Guid id, CreateCategoryDto dto);

    Task DeleteAsync(Guid id);
}