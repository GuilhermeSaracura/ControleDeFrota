using ControleDeFrota.Api.Models;

namespace ControleDeFrota.Api.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<IReadOnlyCollection<Usuario>> ObterTodosAsync();
    Task<Usuario?> ObterPorIdAsync(Guid id);
    Task<Usuario> AdicionarAsync(Usuario usuario);
    Task<Usuario?> AtualizarAsync(Guid id, Usuario usuario);
    Task<bool> RemoverAsync(Guid id);
}
