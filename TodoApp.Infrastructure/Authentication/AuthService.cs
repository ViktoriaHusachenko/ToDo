using TodoApp.Application.DTOs.Auth;
using TodoApp.Application.Interfaces.Repositories;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Security;

namespace TodoApp.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException("Email is already in use.");
        }

        var user = new User
        {
            Email = request.Email,
            DisplayName = request.DisplayName,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            DisplayName = user.DisplayName,
            Email = user.Email
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
                throw new InvalidOperationException("Invalid credentials.");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            DisplayName = user.DisplayName,
            Email = user.Email
        };
    }
}
