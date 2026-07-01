using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Infrastructure.Authentication;
using TodoApp.Infrastructure.Security;

namespace TodoApp.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JwtSettings from appsettings.json (section name defined in JwtSettings)
        var jwtSection = configuration.GetSection(JwtSettings.SectionName);
        var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();
        services.AddSingleton(jwtSettings);

        // Register password hasher
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Register JWT token generator
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
