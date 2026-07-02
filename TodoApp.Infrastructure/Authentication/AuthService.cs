using System.Net;
using TodoApp.Application.DTOs.Auth;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Application.Responses;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Security;

namespace TodoApp.Infrastructure.Authentication;

public class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public async Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existsResp = await userRepository.ExistsByEmailAsync(request.Email);
        if (existsResp.IsError) return Response<AuthResponse>.Error(existsResp.ErrorMessage ?? "An error occurred.");
        if (existsResp.Value) return Response<AuthResponse>.Error("Email is already in use.", HttpStatusCode.BadRequest);

        var user = new UserEntity
        {
            Email = request.Email,
            DisplayName = request.DisplayName,
            PasswordHash = passwordHasher.Hash(request.Password)
        };

        var addResp = await userRepository.AddAsync(user);
        if (addResp.IsError) return Response<AuthResponse>.Error(addResp.ErrorMessage ?? "Failed to add user.");

        var saveResp = await userRepository.SaveChangesAsync();
        if (saveResp.IsError) return Response<AuthResponse>.Error(saveResp.ErrorMessage ?? "Failed to save user.");

        var token = jwtTokenGenerator.GenerateToken(user);

        var auth = new AuthResponse
        {
            Token = token,
            DisplayName = user.DisplayName,
            Email = user.Email
        };

        return Response<AuthResponse>.Ok(auth, HttpStatusCode.Created);
    }

    public async Task<Response<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var userResp = await userRepository.GetByEmailAsync(request.Email);
        if (userResp.IsError) return Response<AuthResponse>.Error(userResp.ErrorMessage ?? "An error occurred.");

        var user = userResp.Value;
        if (user is null) return Response<AuthResponse>.Error("Invalid credentials.", HttpStatusCode.Unauthorized);

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Response<AuthResponse>.Error("Invalid credentials.", HttpStatusCode.Unauthorized);
        }

        var token = jwtTokenGenerator.GenerateToken(user);

        var auth = new AuthResponse
        {
            Token = token,
            DisplayName = user.DisplayName,
            Email = user.Email
        };

        return Response<AuthResponse>.Ok(auth);
    }
}
