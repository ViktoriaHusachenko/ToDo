using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.DTOs.Pagination;
using TodoApp.Application.DTOs.Tasks;
using TodoApp.Application.Interfaces.Services;

namespace TodoApp.API.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    private Guid? GetUserIdFromClaims()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(sub)) return null;
        return Guid.TryParse(sub, out var id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null) return Unauthorized();

        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _taskService.GetPagedAsync(userId.Value, pagination, search, categoryId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _taskService.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null) return Unauthorized();

        var created = await _taskService.CreateAsync(userId.Value, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null) return Unauthorized();

        if (dto.Id == Guid.Empty) dto.Id = id;

        try
        {
            await _taskService.UpdateAsync(userId.Value, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _taskService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
