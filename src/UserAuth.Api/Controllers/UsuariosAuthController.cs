using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserAuth.Application.Contracts;
using UserAuth.Application.DTOs.Auth;
using UserAuth.Application.Notifications;

namespace UserAuth.Api.Controllers;

[AllowAnonymous]
[Route("auth/[controller]")]
public class UsuariosAuthController : BaseController
{
    private readonly IAuthService _authService;
    
    public UsuariosAuthController(INotificator notificator, IAuthService authService) : base(notificator)
    {
        _authService = authService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Login", Tags = new[] { "Usuário - Auth" })]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
    {
        var usuario = await _authService.Login(dto);
        return usuario != null ? OkResponse(usuario) : Unauthorized(new[] { "Usuario e/ou senha incorretos" });
    }
}