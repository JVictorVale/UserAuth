using Microsoft.OpenApi.Models;

namespace UserAuth.Api.Configuration;

public static class SwaggerConfiguration
{
    public static void AddSwagger(this IServiceCollection services)
    {
        var contact = new OpenApiContact
        {
            Name = "Victor Vale",
            Email = "joaovictorvale.dev@gmail.com\n",
            Url = new Uri("https://github.com/jvictorvale")
        };
        
        var license = new OpenApiLicense
        {
            Name = "Free License",
            Url = new Uri("https://github.com/jvictorvale")
        };
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Version = "v1",
                Title = "Autenticação de Usuario API",
                Contact = contact,
                License = license
            });

            options.OrderActionsBy(a => a.GroupName);
            
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security\n Insira o token JWT desta maneira: Bearer {seu token}",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}