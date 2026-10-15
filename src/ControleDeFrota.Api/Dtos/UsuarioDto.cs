using ControleDeFrota.Api.Models;

namespace ControleDeFrota.Api.Dtos;

/// <summary>
/// Representa os dados enviados na criação de um usuário.
/// </summary>
public class UsuarioRequestDto
{
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public string Departamento { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public TipoContratacao TipoContratacao { get; set; }
    public string NumeroHabilitacao { get; set; } = string.Empty;
    public CategoriaHabilitacao CategoriasHabilitacao { get; set; } = CategoriaHabilitacao.Nenhuma;
    public Guid? VeiculoId { get; set; }
}

/// <summary>
/// Representa os dados retornados em respostas relacionadas a usuários.
/// </summary>
public class UsuarioResponseDto
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public string Departamento { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public TipoContratacao TipoContratacao { get; set; }
    public string NumeroHabilitacao { get; set; } = string.Empty;
    public CategoriaHabilitacao CategoriasHabilitacao { get; set; } = CategoriaHabilitacao.Nenhuma;
    public Guid? VeiculoId { get; set; }
}

/// <summary>
/// Realiza a conversão entre DTOs e a entidade de usuário.
/// </summary>
public static class UsuarioMapper
{
    /// <summary>
    /// Converte a requisição de criação para a entidade interna de usuário.
    /// </summary>
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

    /// <summary>
    /// Converte a entidade de usuário em um DTO de resposta.
    /// </summary>
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
