using Microsoft.EntityFrameworkCore;
using UserAuth.Domain.Contracts.Repositories;
using UserAuth.Domain.Entities;
using UserAuth.Infra.Context;

namespace UserAuth.Infra.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Cadastrar(Usuario usuario)
    {
        Context.Usuarios.Add(usuario);
    }

    public void Atualizar(Usuario usuario)
    {
        Context.Usuarios.Update(usuario);
    }

    public async Task<Usuario?> ObterPorId(int id)
    {
        return await Context.Usuarios.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> ObterPorEmail(string email)
    {
        return await Context.Usuarios.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<Usuario>> ObterTodos()
    {
        return await Context.Usuarios.AsNoTrackingWithIdentityResolution().ToListAsync();
    }

    public async Task<Usuario?> ObterPorTokenDeVerificacao(string token)
    {
        return await Context.Usuarios.FirstOrDefaultAsync(u => u.TokenDeVerificacao == token);
    }

    public async Task<Usuario?> ObterPorTokenDeResetSenha(string token)
    {
        return await Context.Usuarios.FirstOrDefaultAsync(u => u.TokenDeResetSenha == token);
    }
    
    public async Task<Usuario?> ObterPedidoResetSenhaValido(int id)
    {
        return await Context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.ExpiraResetToken > DateTime.Now);
    }
}