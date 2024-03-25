using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ScottBrady91.AspNetCore.Identity;
using UserAuth.Application.Contracts;
using UserAuth.Application.Email;
using UserAuth.Application.Notifications;
using UserAuth.Application.Services;
using UserAuth.Core.Enums;
using UserAuth.Core.Extensions;
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
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
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
            .AddScoped<IUsuarioService, UsuarioService>()
            .AddScoped<IFileService, FileService>()
            .AddScoped<IEmailService, EmailService>();
    }
    
    public static void UseStaticFileConfiguration(this IApplicationBuilder app, IConfiguration configuration)
    {
        var uploadSettings = configuration.GetSection("UploadSettings");
        var publicBasePath = uploadSettings.GetValue<string>("PublicBasePath");
        var privateBasePath = uploadSettings.GetValue<string>("PrivateBasePath");
    
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(publicBasePath),
            RequestPath = $"/{EPathAccess.Public.ToDescriptionString()}"
        });
    
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(privateBasePath),
            RequestPath = $"/{EPathAccess.Private.ToDescriptionString()}",
            OnPrepareResponse = ctx =>
            {
                // respond HTTP 401 Unauthorized.
                ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                ctx.Context.Response.ContentLength = 0;
                ctx.Context.Response.Body = Stream.Null;
                ctx.Context.Response.Headers.Add("Cache-Control", "no-store");
            }
        });
    }
}