using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserAuth.Application.Contracts;
using UserAuth.Application.DTOs.Auth;
using UserAuth.Application.DTOs.Usuario;
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
    
    [HttpPost("cadastrar")]
    [SwaggerOperation(Summary = "Cadastro de um Usuário", Tags = new[] { "Usuário - Auth" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] RegistrarUsuarioDto dto)
    {
        return OkResponse(await _authService.Cadastrar(dto));
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login", Tags = new[] { "Usuário - Auth" })]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginUsuarioDto dto)
    {
        var usuario = await _authService.Login(dto);
        return usuario != null ? OkResponse(usuario) : Unauthorized(new[] { "Usuario e/ou senha incorretos" });
    }

    [HttpGet("verificar-conta")]
    [SwaggerOperation(Summary = "Verificar e-mail do usuário", Tags = new[] { "Usuário - Auth" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerificarEmail([FromQuery] string token, [FromQuery] string email)
    { 
        await _authService.VerificarEmail(token, email);
        return OkResponse("E-mail verificado com sucesso");
    }
    
    [HttpPost("esqueceu-senha")]
    [SwaggerOperation(Summary = "Esqueceu a Senha", Tags = new[] { "Usuário - Auth" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EsqueceuSenha([FromBody] string email)
    {
        await _authService.EsqueceuSenha(email);
        return OkResponse("Um e-mail foi enviado com instruções para redefinir sua senha.");
    }

    [HttpPost("resetar-senha")]
    [SwaggerOperation(Summary = "Redefinir Senha", Tags = new[] { "Usuário - Auth" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetSenha([FromBody] ResetSenhaDto requestDto)
    {
        await _authService.ResetSenha(requestDto);
        return OkResponse("Senha alterada com sucesso.");
    }
}