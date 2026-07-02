using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using TodoApp.Application.Interfaces.Services;

namespace TodoApp.API.Security;

public class CurrentUserContext(IHttpContextAccessor accessor) : ICurrentUserContext
{
    private readonly HttpContext _context = accessor.HttpContext;

    private ClaimsPrincipal? User => _context.User;

    public Guid? Id
    {
        get
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier);
            var sub = claim?.Value;
            if (string.IsNullOrWhiteSpace(sub)) return null;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value ?? User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Name => User?.FindFirst("displayName")?.Value ?? User?.FindFirst(JwtRegisteredClaimNames.Name)?.Value ?? User?.FindFirst(ClaimTypes.Name)?.Value;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
