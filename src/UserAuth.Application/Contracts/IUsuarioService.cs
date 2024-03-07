using UserAuth.Application.DTOs.Usuario;

namespace UserAuth.Application.Contracts;

public interface IUsuarioService
{
    Task<UsuarioDto?> Adicionar(AdicionarUsuarioDto usuarioDto);
    Task<UsuarioDto?> Atualizar(int id, AtualizarUsuarioDto usuarioDto);
    Task<UsuarioDto?> ObterPorId(int id);
    Task<List<UsuarioDto>?> ObterTodos();
}