using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserAuth.Application.Contracts;
using UserAuth.Application.DTOs.Usuario;
using UserAuth.Application.Notifications;

namespace UserAuth.Api.Controllers;

[Route("/[controller]")]
public class UsuariosController : BaseController
{
    private readonly IUsuarioService _usuarioService;
    
    public UsuariosController(INotificator notificator, IUsuarioService usuarioService) : base(notificator)
    {
        _usuarioService = usuarioService;
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar um Usuário", Tags = new[] { "Usuário - Usuarios" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromForm] AtualizarUsuarioDto dto)
    {
        return OkResponse(await _usuarioService.Atualizar(id, dto));
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter um Usuário", Tags = new[] { "Usuário - Usuarios" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        return OkResponse(await _usuarioService.ObterPorId(id));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Obter todos", Tags = new[] { "Usuário - Usuarios" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterTodos()
    {
        return OkResponse(await _usuarioService.ObterTodos());
    }
}