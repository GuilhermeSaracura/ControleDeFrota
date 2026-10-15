using System.Collections.Concurrent;
using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories.Interfaces;

namespace ControleDeFrota.Api.Repositories;

public class InMemoryVeiculoRepository : IVeiculoRepository
{
    private readonly ConcurrentDictionary<Guid, Veiculo> _veiculos = new();

    public InMemoryVeiculoRepository()
    {
        var veiculosSeed = new[]
        {
            new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = "ABC-1234",
                Modelo = "Fiat Uno",
                Ano = 2022,
                Cor = "Branco",
                Disponivel = true,
                Categoria = CategoriaHabilitacao.B,
                UsuarioId = null
            },
            new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = "DEF-5678",
                Modelo = "Chevrolet Onix",
                Ano = 2023,
                Cor = "Prata",
                Disponivel = false,
                Categoria = CategoriaHabilitacao.C,
                UsuarioId = null
            },
            new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = "IJF-0129",
                Modelo = "Fiat Uno",
                Ano = 2022,
                Cor = "Vermelho",
                Disponivel = true,
                Categoria = CategoriaHabilitacao.B,
                UsuarioId = null
            }
        };

        foreach (var veiculo in veiculosSeed)
        {
            _veiculos[veiculo.Id] = veiculo;
        }
    }

    public Task<IReadOnlyCollection<Veiculo>> ObterTodosAsync()
    {
        var veiculos = _veiculos.Values
            .OrderBy(v => v.Modelo)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<Veiculo>>(veiculos);
    }

    public Task<Veiculo?> ObterPorIdAsync(Guid id)
    {
        _veiculos.TryGetValue(id, out var veiculo);
        return Task.FromResult(veiculo);
    }

    public Task<Veiculo> AdicionarAsync(Veiculo veiculo)
    {
        if (veiculo.Id == Guid.Empty)
        {
            veiculo.Id = Guid.NewGuid();
        }

        _veiculos[veiculo.Id] = veiculo;
        return Task.FromResult(veiculo);
    }

    public Task<Veiculo?> AtualizarAsync(Guid id, Veiculo veiculo)
    {
        if (!_veiculos.ContainsKey(id))
        {
            return Task.FromResult<Veiculo?>(null);
        }

        veiculo.Id = id;
        _veiculos[id] = veiculo;

        return Task.FromResult<Veiculo?>(veiculo);
    }

    public Task<bool> RemoverAsync(Guid id)
    {
        return Task.FromResult(_veiculos.TryRemove(id, out _));
    }
}
