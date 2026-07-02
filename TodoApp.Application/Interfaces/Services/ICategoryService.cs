using TodoApp.Application.DTOs.Categories;
using TodoApp.Application.Responses;

namespace TodoApp.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<Response<IEnumerable<CategoryDto>>> GetAllAsync(Guid userId);
    Task<Response<CategoryDto?>> GetByIdAsync(Guid id);

    Task<Response<CategoryDto>> CreateAsync(Guid userId, CreateCategoryDto dto);

    Task<Response> UpdateAsync(Guid id, CreateCategoryDto dto);

    Task<Response> DeleteAsync(Guid id);
}
