using ControleDeFrota.Api.Models;

namespace ControleDeFrota.Api.Dtos;

public class UsuarioRequestDto
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Departamento { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public TipoContratacao TipoContratacao { get; set; }
    public string NumeroHabilitacao { get; set; } = string.Empty;
    public CategoriaHabilitacao CategoriasHabilitacao { get; set; } = CategoriaHabilitacao.Nenhuma;
    public Guid? VeiculoId { get; set; }
}

public class UsuarioResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Departamento { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public TipoContratacao TipoContratacao { get; set; }
    public string NumeroHabilitacao { get; set; } = string.Empty;
    public CategoriaHabilitacao CategoriasHabilitacao { get; set; } = CategoriaHabilitacao.Nenhuma;
    public Guid? VeiculoId { get; set; }
}

public static class UsuarioMapper
{
    public static Usuario ToEntity(this UsuarioRequestDto dto)
    {
        return new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Departamento = dto.Departamento,
            Ativo = dto.Ativo,
            TipoContratacao = dto.TipoContratacao,
            NumeroHabilitacao = dto.NumeroHabilitacao,
            CategoriasHabilitacao = dto.CategoriasHabilitacao,
            VeiculoId = dto.VeiculoId
        };
    }

    public static UsuarioResponseDto ToResponse(this Usuario usuario)
    {
        return new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Departamento = usuario.Departamento,
            Ativo = usuario.Ativo,
            TipoContratacao = usuario.TipoContratacao,
            NumeroHabilitacao = usuario.NumeroHabilitacao,
            CategoriasHabilitacao = usuario.CategoriasHabilitacao,
            VeiculoId = usuario.VeiculoId
        };
    }
}
