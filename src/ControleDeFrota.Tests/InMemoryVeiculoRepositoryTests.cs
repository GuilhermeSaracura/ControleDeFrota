using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories;

namespace ControleDeFrota.Tests;

public class InMemoryVeiculoRepositoryTests
{
    [Fact]
    public async Task ObterTodosAsync_DeveRetornarVeiculosSeedados()
    {
        var repository = new InMemoryVeiculoRepository();

        var veiculos = await repository.ObterTodosAsync();

        Assert.NotEmpty(veiculos);
        Assert.Contains(veiculos, veiculo => veiculo.Placa == "ABC-1234");
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
            Disponivel = true
        };

        await repository.AdicionarAsync(veiculo);

        var resultado = await repository.ObterPorIdAsync(veiculo.Id);

        Assert.NotNull(resultado);
        Assert.Equal("GHI-9012", resultado!.Placa);
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
            Disponivel = true
        };

        await repository.AdicionarAsync(veiculo);
        var removido = await repository.RemoverAsync(veiculo.Id);

        Assert.True(removido);
        Assert.Null(await repository.ObterPorIdAsync(veiculo.Id));
    }
}
