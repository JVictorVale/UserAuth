using System.Globalization;
using Microsoft.AspNetCore.Localization;
using UserAuth.Api.Configuration;
using UserAuth.Application;

var builder = WebApplication.CreateBuilder(args);


builder
    .Services
    .Configure<RequestLocalizationOptions>(o => 
    {
        var supportedCultures = new[] { new CultureInfo("pt-BR") };
        o.DefaultRequestCulture = new RequestCulture("pt-BR", "pt-BR");
        o.SupportedCultures = supportedCultures;
        o.SupportedUICultures = supportedCultures;
    });

builder
    .Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Services.AddAuthenticationConfig(builder.Configuration);

builder.Services.SetupSettings(builder.Configuration);
builder.Services.ConfigureApplication(builder.Configuration);
builder
    .Services
    .AddSwagger();

builder.Services.AddRouting(c => c.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyHeader();
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFileConfiguration(builder.Configuration);

app.MapControllers();

app.Run();