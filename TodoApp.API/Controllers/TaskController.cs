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
    private readonly ICurrentUserContext _currentUser;

    public TaskController(ITaskService taskService, ICurrentUserContext currentUser)
    {
        _taskService = taskService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Id is null) return Unauthorized();

        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var resp = await _taskService.GetPagedAsync(_currentUser.Id.Value, pagination, search, categoryId);
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
        if (!_currentUser.IsAuthenticated || _currentUser.Id is null) return Unauthorized();

        var resp = await _taskService.CreateAsync(_currentUser.Id.Value, dto);
        return ToResponse(resp);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Id is null) return Unauthorized();

        if (dto.Id == Guid.Empty) dto.Id = id;
        var resp = await _taskService.UpdateAsync(_currentUser.Id.Value, dto);
        return ToResponse(resp);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var resp = await _taskService.DeleteAsync(id);
        return ToResponse(resp);
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var response = await _taskService.CompleteAsync(id);
        return ToResponse(response);
    }

    [HttpPatch("{id}/uncomplete")]
    public async Task<IActionResult> Uncomplete(Guid id)
    {
        var response = await _taskService.UncompleteAsync(id);
        return ToResponse(response);
    }
}
