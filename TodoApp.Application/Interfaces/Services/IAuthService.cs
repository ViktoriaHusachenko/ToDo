using TodoApp.Application.DTOs.Auth;
using TodoApp.Application.Responses;

namespace TodoApp.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request);

    Task<Response<AuthResponse>> LoginAsync(LoginRequest request);
}
