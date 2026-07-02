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
    private readonly ICurrentUserContext _currentUser;

    public CategoryController(ICategoryService categoryService, ICurrentUserContext currentUser)
    {
        _categoryService = categoryService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Id is null) return Unauthorized();

        var resp = await _categoryService.GetAllAsync(_currentUser.Id.Value);
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
        if (!_currentUser.IsAuthenticated || _currentUser.Id is null) return Unauthorized();

        var resp = await _categoryService.CreateAsync(_currentUser.Id.Value, dto);
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
