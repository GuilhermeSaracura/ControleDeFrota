using ControleDeFrota.Api.Models;

namespace ControleDeFrota.Api.Dtos;

/// <summary>
/// Representa os dados enviados para cadastrar um veículo.
/// </summary>
public class VeiculoRequestDto
{
    public required string Placa { get; set; }
    public required string Modelo { get; set; }
    public int Ano { get; set; }
    public required string Cor { get; set; }
    public bool Disponivel { get; set; } = true;
    public CategoriaHabilitacao Categoria { get; set; }
}

/// <summary>
/// Representa os dados enviados para atualizar um veículo, incluindo o usuário vinculado.
/// </summary>
public class VeiculoUpdateRequestDto
{
    public required string Placa { get; set; }
    public required string Modelo { get; set; }
    public int Ano { get; set; }
    public required string Cor { get; set; }
    public bool Disponivel { get; set; }
    public CategoriaHabilitacao Categoria { get; set; }
    public Guid? UsuarioId { get; set; }
}

/// <summary>
/// Representa o retorno completo de um veículo na API.
/// </summary>
public class VeiculoResponseDto
{
    public Guid Id { get; set; }
    public required string Placa { get; set; }
    public required string Modelo { get; set; }
    public int Ano { get; set; }
    public required string Cor { get; set; }
    public bool Disponivel { get; set; }
    public CategoriaHabilitacao Categoria { get; set; }
    public Guid? UsuarioId { get; set; }
}

/// <summary>
/// Realiza a conversão entre DTOs e a entidade de veículo.
/// </summary>
public static class VeiculoMapper
{
    /// <summary>
    /// Converte a requisição de criação para a entidade interna de veículo.
    /// </summary>
    public static Veiculo ToEntity(this VeiculoRequestDto dto)
    {
        return new Veiculo
        {
            Placa = dto.Placa,
            Modelo = dto.Modelo,
            Ano = dto.Ano,
            Cor = dto.Cor,
            Disponivel = dto.Disponivel,
            Categoria = dto.Categoria,
            UsuarioId = null
        };
    }

    /// <summary>
    /// Converte a requisição de atualização para a entidade interna de veículo.
    /// </summary>
    public static Veiculo ToEntity(this VeiculoUpdateRequestDto dto)
    {
        return new Veiculo
        {
            Placa = dto.Placa,
            Modelo = dto.Modelo,
            Ano = dto.Ano,
            Cor = dto.Cor,
            Disponivel = dto.Disponivel,
            Categoria = dto.Categoria,
            UsuarioId = dto.UsuarioId
        };
    }

    /// <summary>
    /// Converte a entidade em um DTO de resposta.
    /// </summary>
    public static VeiculoResponseDto ToResponse(this Veiculo veiculo)
    {
        return new VeiculoResponseDto
        {
            Id = veiculo.Id,
            Placa = veiculo.Placa,
            Modelo = veiculo.Modelo,
            Ano = veiculo.Ano,
            Cor = veiculo.Cor,
            Disponivel = veiculo.Disponivel,
            Categoria = veiculo.Categoria,
            UsuarioId = veiculo.UsuarioId
        };
    }
}
