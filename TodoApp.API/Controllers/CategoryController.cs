using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.DTOs.Categories;
using TodoApp.Application.Interfaces.Services;

namespace TodoApp.API.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoryController : ApiControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    private Guid? GetUserIdFromClaims()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(sub)) return null;
        return Guid.TryParse(sub, out var id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserIdFromClaims();
        if (userId is null) return Unauthorized();

        var resp = await _categoryService.GetAllAsync(userId.Value);
        return ToResponse(resp);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var resp = await _categoryService.GetByIdAsync(id);
        return ToResponse(resp);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null) return Unauthorized();

        var resp = await _categoryService.CreateAsync(userId.Value, dto);
        return ToResponse(resp);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateCategoryDto dto)
    {
        var resp = await _categoryService.UpdateAsync(id, dto);
        return ToResponse(resp);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var resp = await _categoryService.DeleteAsync(id);
        return ToResponse(resp);
    }
}
