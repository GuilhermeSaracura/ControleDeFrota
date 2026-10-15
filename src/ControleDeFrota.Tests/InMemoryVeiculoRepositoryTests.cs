using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories;
using FluentAssertions;

namespace ControleDeFrota.Tests;

public class InMemoryVeiculoRepositoryTests
{
    [Fact]
    public async Task ObterTodosAsync_DeveRetornarVeiculosSeedados()
    {
        var repository = new InMemoryVeiculoRepository();

        var veiculos = await repository.ObterTodosAsync();

        veiculos.Should().NotBeEmpty();
        veiculos.Should().Contain(veiculo => veiculo.Placa == "ABC-1234");
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarNovoVeiculo()
    {
        var repository = new InMemoryVeiculoRepository();
        var veiculo = new Veiculo
        {
            Placa = "GHI-9012",
            Modelo = "Toyota Corolla",
            Ano = 2024,
            Cor = "Azul",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B
        };

        await repository.AdicionarAsync(veiculo);

        var resultado = await repository.ObterPorIdAsync(veiculo.Id);

        resultado.Should().NotBeNull();
        resultado!.Placa.Should().Be("GHI-9012");
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverVeiculoExistente()
    {
        var repository = new InMemoryVeiculoRepository();
        var veiculo = new Veiculo
        {
            Placa = "JKL-3456",
            Modelo = "Honda Civic",
            Ano = 2021,
            Cor = "Preto",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.C
        };

        await repository.AdicionarAsync(veiculo);
        var removido = await repository.RemoverAsync(veiculo.Id);

        removido.Should().BeTrue();
        (await repository.ObterPorIdAsync(veiculo.Id)).Should().BeNull();
    }
}
