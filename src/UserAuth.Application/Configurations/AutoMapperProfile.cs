using AutoMapper;
using UserAuth.Application.DTOs.Usuario;
using UserAuth.Core.Extensions;
using UserAuth.Domain.Entities;

namespace UserAuth.Application.Configurations;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Usuario, UsuarioDto>().ReverseMap()
            .AfterMap((_, dest) => dest.Cpf = dest.Cpf.SomenteNumeros()!);
        CreateMap<Usuario, AdicionarUsuarioDto>().ReverseMap()
            .AfterMap((_, dest) => dest.Cpf = dest.Cpf.SomenteNumeros()!);
        CreateMap<Usuario, AtualizarUsuarioDto>().ReverseMap()
            .AfterMap((_, dest) => dest.Cpf = dest.Cpf.SomenteNumeros()!);
    }
}