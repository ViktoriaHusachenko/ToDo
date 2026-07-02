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
public class TaskController : ApiControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    private Guid? GetUserIdFromClaims()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(sub))
        {
            // fallback to mapped name identifier claim
            sub = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        }

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

        var resp = await _taskService.GetPagedAsync(userId.Value, pagination, search, categoryId);
        return ToResponse(resp);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var resp = await _taskService.GetByIdAsync(id);
        return ToResponse(resp);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null) return Unauthorized();

        var resp = await _taskService.CreateAsync(userId.Value, dto);
        return ToResponse(resp);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null) return Unauthorized();

        if (dto.Id == Guid.Empty) dto.Id = id;
        var resp = await _taskService.UpdateAsync(userId.Value, dto);
        return ToResponse(resp);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var resp = await _taskService.DeleteAsync(id);
        return ToResponse(resp);
    }
}
