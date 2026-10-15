using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Utils;
using FluentAssertions;

namespace ControleDeFrota.Tests;

public class RegraFrotaUtilsTests
{
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
            TipoContratacao = tipoContratacao,
            CategoriasHabilitacao = categoriasHabilitacao
        };
        var veiculo = new Veiculo { Categoria = categoriaVeiculo };

        var resultado = RegraFrotaUtils.PossuiCategoriaParaVeiculo(usuario, veiculo);

        resultado.Should().BeTrue();
    }

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
            TipoContratacao = tipoContratacao,
            CategoriasHabilitacao = categoriasHabilitacao
        };
        var veiculo = new Veiculo { Categoria = categoriaVeiculo };

        var resultado = RegraFrotaUtils.PossuiCategoriaParaVeiculo(usuario, veiculo);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void ValidarVinculoVeiculo_DeveRetornarErroQuandoVeiculoNaoExiste()
    {
        var usuario = new Usuario
        {
            TipoContratacao = TipoContratacao.Funcionario,
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = Guid.NewGuid()
        };

        var resultado = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, null);

        resultado.Should().Be("Veículo informado não foi encontrado.");
    }

    [Fact]
    public void ValidarVinculoVeiculo_DeveRetornarErroQuandoCategoriaNaoCompativel()
    {
        var usuario = new Usuario
        {
            TipoContratacao = TipoContratacao.Funcionario,
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = Guid.NewGuid()
        };
        var veiculo = new Veiculo
        {
            Id = Guid.NewGuid(),
            Categoria = CategoriaHabilitacao.C,
            UsuarioId = usuario.Id
        };

        var resultado = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, veiculo);

        resultado.Should().Be("Funcionário não está habilitado para a categoria do veículo informado.");
    }

    [Fact]
    public void ValidarVinculoVeiculo_DeveRetornarErroQuandoUsuarioJaEstaVinculadoAOutroVeiculo()
    {
        var usuario = new Usuario
        {
            TipoContratacao = TipoContratacao.Funcionario,
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = Guid.NewGuid()
        };
        var veiculo = new Veiculo
        {
            Id = Guid.NewGuid(),
            Categoria = CategoriaHabilitacao.B,
            UsuarioId = usuario.Id
        };

        var resultado = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, veiculo);

        resultado.Should().Be("Funcionário já está vinculado a outro veículo.");
    }

    [Fact]
    public void ValidarVinculoVeiculo_DeveRetornarNullQuandoVinculoForValido()
    {
        var usuarioId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();
        var usuario = new Usuario
        {
            Id = usuarioId,
            TipoContratacao = TipoContratacao.Funcionario,
            CategoriasHabilitacao = CategoriaHabilitacao.BEC,
            VeiculoId = veiculoId
        };
        var veiculo = new Veiculo
        {
            Id = veiculoId,
            Categoria = CategoriaHabilitacao.B,
            UsuarioId = usuarioId
        };

        var resultado = RegraFrotaUtils.ValidarVinculoVeiculo(usuario, veiculo);

        resultado.Should().BeNull();
    }
}
