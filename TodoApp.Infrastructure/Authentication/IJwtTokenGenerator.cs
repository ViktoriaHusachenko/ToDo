using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}