using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Utils;
using FluentAssertions;

namespace ControleDeFrota.Tests;

public class RegraFrotaUtilsTests
{
    /// <summary>
    /// Verifica que a categoria do veículo é aceita quando o usuário possui habilitação compatível.
    /// </summary>
    [Theory]
    [InlineData(TipoContratacao.Funcionario, CategoriaHabilitacao.B, CategoriaHabilitacao.B)]
    [InlineData(TipoContratacao.Funcionario, CategoriaHabilitacao.BEC, CategoriaHabilitacao.C)]
    [InlineData(TipoContratacao.Estagiario, CategoriaHabilitacao.C, CategoriaHabilitacao.C)]
    [InlineData(TipoContratacao.Aprendiz, CategoriaHabilitacao.BEC, CategoriaHabilitacao.B)]
    public void PossuiCategoriaParaVeiculo_DeveRetornarTrue_QuandoUsuarioTemCategoriaCompativel(
        TipoContratacao tipoContratacao,
        CategoriaHabilitacao categoriasHabilitacao,
        CategoriaHabilitacao categoriaVeiculo)
    {
        var usuario = new Usuario
        {
            Nome = "Usuário Teste",
            Email = "usuario@empresa.com",
            TipoContratacao = tipoContratacao,
            CategoriasHabilitacao = categoriasHabilitacao
        };
        var veiculo = new Veiculo
        {
            Placa = "ABC-1234",
            Modelo = "Fiat Uno",
            Cor = "Branco",
            Categoria = categoriaVeiculo
        };

        var resultado = RegraFrotaUtils.PossuiCategoriaParaVeiculo(usuario, veiculo);

        resultado.Should().BeTrue();
    }

    /// <summary>
    /// Verifica que a categoria do veículo é rejeitada quando não existe compatibilidade com o usuário.
    /// </summary>
    [Theory]
    [InlineData(TipoContratacao.Funcionario, CategoriaHabilitacao.B, CategoriaHabilitacao.C)]
    [InlineData(TipoContratacao.Estagiario, CategoriaHabilitacao.B, CategoriaHabilitacao.C)]
    [InlineData(TipoContratacao.Aprendiz, CategoriaHabilitacao.C, CategoriaHabilitacao.B)]
    public void PossuiCategoriaParaVeiculo_DeveRetornarFalse_QuandoCategoriaNaoForCompativel(
        TipoContratacao tipoContratacao,
        CategoriaHabilitacao categoriasHabilitacao,
        CategoriaHabilitacao categoriaVeiculo)
    {
        var usuario = new Usuario
        {
            Nome = "Usuário Teste",
            Email = "usuario@empresa.com",
            TipoContratacao = tipoContratacao,
            CategoriasHabilitacao = categoriasHabilitacao
        };
        var veiculo = new Veiculo
        {
            Placa = "DEF-5678",
            Modelo = "Chevrolet Onix",
            Cor = "Prata",
            Categoria = categoriaVeiculo
        };

        var resultado = RegraFrotaUtils.PossuiCategoriaParaVeiculo(usuario, veiculo);

        resultado.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que a ausência de veículo gera erro explícito de validação.
    /// </summary>
    [Fact]
    public void ValidarVinculoVeiculo_DeveRetornarErroQuandoVeiculoNaoExiste()
    {
        var usuario = new Usuario
        {
            Nome = "Maria Silva",
            Email = "maria@empresa.com",
            TipoContratacao = TipoContratacao.Funcionario,
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = Guid.NewGuid()
        };

        var resultado = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, null);

        resultado.Should().Be("Veículo informado não foi encontrado.");
    }

    /// <summary>
    /// Verifica que o sistema rejeita veículos fora da categoria habilitada do usuário.
    /// </summary>
    [Fact]
    public void ValidarVinculoVeiculo_DeveRetornarErroQuandoCategoriaNaoCompativel()
    {
        var usuario = new Usuario
        {
            Nome = "João Silva",
            Email = "joao@empresa.com",
            TipoContratacao = TipoContratacao.Funcionario,
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = Guid.NewGuid()
        };
        var veiculo = new Veiculo
        {
            Id = Guid.NewGuid(),
            Placa = "GHI-9012",
            Modelo = "Toyota Corolla",
            Cor = "Azul",
            Categoria = CategoriaHabilitacao.C,
            UsuarioId = usuario.Id
        };

        var resultado = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, veiculo);

        resultado.Should().Be("Funcionário não está habilitado para a categoria do veículo informado.");
    }

    /// <summary>
    /// Verifica que o vínculo com outro veículo é bloqueado quando o usuário já está associado a um veículo diferente.
    /// </summary>
    [Fact]
    public void ValidarVinculoVeiculo_DeveRetornarErroQuandoUsuarioJaEstaVinculadoAOutroVeiculo()
    {
        var usuario = new Usuario
        {
            Nome = "Carlos Souza",
            Email = "carlos@empresa.com",
            TipoContratacao = TipoContratacao.Funcionario,
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = Guid.NewGuid()
        };
        var veiculo = new Veiculo
        {
            Id = Guid.NewGuid(),
            Placa = "JKL-3456",
            Modelo = "Honda Civic",
            Cor = "Preto",
            Categoria = CategoriaHabilitacao.B,
            UsuarioId = usuario.Id
        };

        var resultado = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, veiculo);

        resultado.Should().Be("Funcionário já está vinculado a outro veículo.");
    }

    /// <summary>
    /// Verifica que o vínculo válido entre usuário e veículo não gera erro.
    /// </summary>
    [Fact]
    public void ValidarVinculoVeiculo_DeveRetornarNullQuandoVinculoForValido()
    {
        var usuarioId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();
        var usuario = new Usuario
        {
            Id = usuarioId,
            Nome = "Fernanda Costa",
            Email = "fernanda@empresa.com",
            TipoContratacao = TipoContratacao.Funcionario,
            CategoriasHabilitacao = CategoriaHabilitacao.BEC,
            VeiculoId = veiculoId
        };
        var veiculo = new Veiculo
        {
            Id = veiculoId,
            Placa = "LMN-6789",
            Modelo = "Volkswagen Gol",
            Cor = "Cinza",
            Categoria = CategoriaHabilitacao.B,
            UsuarioId = usuarioId
        };

        var resultado = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, veiculo);

        resultado.Should().BeNull();
    }
}
