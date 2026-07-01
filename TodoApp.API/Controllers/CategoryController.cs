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
public class CategoryController : ControllerBase
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

        var items = await _categoryService.GetAllAsync(userId.Value);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _categoryService.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null) return Unauthorized();

        try
        {
            var created = await _categoryService.CreateAsync(userId.Value, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateCategoryDto dto)
    {
        try
        {
            await _categoryService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
