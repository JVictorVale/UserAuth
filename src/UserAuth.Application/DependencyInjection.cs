using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottBrady91.AspNetCore.Identity;
using UserAuth.Application.Contracts;
using UserAuth.Application.Notifications;
using UserAuth.Application.Services;
using UserAuth.Core.Settings;
using UserAuth.Domain.Entities;
using UserAuth.Infra;

namespace UserAuth.Application;

public static class DependencyInjection
{
    public static void SetupSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
    }
    
    public static void ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureDbContext(configuration);
        
        services.AddDependencyRepositories();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        AddDependencyServices(services);
    }
    
    private static void AddDependencyServices(this IServiceCollection services)
    {
        services
            .AddScoped<INotificator, Notificator>()
            .AddScoped<IPasswordHasher<Usuario>, Argon2PasswordHasher<Usuario>>();

        services
            .AddScoped<IAuthService, AuthService>() 
            .AddScoped<IUsuarioService, UsuarioService>();
    }
}