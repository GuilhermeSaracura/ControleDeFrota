using ControleDeFrota.Api.Models;

namespace ControleDeFrota.Api.Dtos;

public class VeiculoRequestDto
{
    public required string Placa { get; set; }
    public required string Modelo { get; set; }
    public int Ano { get; set; }
    public required string Cor { get; set; }
    public bool Disponivel { get; set; } = true;
    public CategoriaHabilitacao Categoria { get; set; }
}

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

public static class VeiculoMapper
{
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
