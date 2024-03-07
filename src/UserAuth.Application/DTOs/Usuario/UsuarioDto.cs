﻿namespace UserAuth.Application.DTOs.Usuario;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}