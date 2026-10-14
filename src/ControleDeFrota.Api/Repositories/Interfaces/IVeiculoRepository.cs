using ControleDeFrota.Api.Models;

namespace ControleDeFrota.Api.Repositories.Interfaces;

public interface IVeiculoRepository
{
    Task<IReadOnlyCollection<Veiculo>> ObterTodosAsync();
    Task<Veiculo?> ObterPorIdAsync(Guid id);
    Task<Veiculo> AdicionarAsync(Veiculo veiculo);
    Task<Veiculo?> AtualizarAsync(Guid id, Veiculo veiculo);
    Task<bool> RemoverAsync(Guid id);
}
