using System.Net;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Responses;

namespace TodoApp.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToResponse(Response response)
    {
        if (response is null) return StatusCode((int)HttpStatusCode.InternalServerError);

        if (response.IsError)
        {
            return StatusCode((int)response.StatusCode, new { error = response.ErrorMessage });
        }

        return response.StatusCode switch
        {
            HttpStatusCode.NoContent => NoContent(),
            _ => StatusCode((int)response.StatusCode)
        };
    }

    protected IActionResult ToResponse<T>(Response<T> response)
    {
        if (response is null) return StatusCode((int)HttpStatusCode.InternalServerError);

        if (response.IsError)
        {
            return StatusCode((int)response.StatusCode, new { error = response.ErrorMessage });
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return NoContent();
        }

        return StatusCode((int)response.StatusCode, response.Value);
    }
}
