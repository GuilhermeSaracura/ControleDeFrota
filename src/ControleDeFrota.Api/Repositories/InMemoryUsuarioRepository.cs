using System.Collections.Concurrent;
using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories.Interfaces;

namespace ControleDeFrota.Api.Repositories;

public class InMemoryUsuarioRepository : IUsuarioRepository
{
    private readonly ConcurrentDictionary<Guid, Usuario> _usuarios = new();

    public InMemoryUsuarioRepository()
    {
        var usuariosSeed = new[]
        {
            new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Maria Silva",
                Email = "maria@empresa.com",
                Departamento = "Logística",
                Ativo = true,
                TipoContratacao = TipoContratacao.Funcionario,
                PossuiHabilitacao = true,
                NumeroHabilitacao = "12345678901",
                CategoriasHabilitacao = CategoriaHabilitacao.B,
                VeiculoId = null
            },
            new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "João Pereira",
                Email = "joao@empresa.com",
                Departamento = "Operações",
                Ativo = true,
                TipoContratacao = TipoContratacao.Funcionario,
                PossuiHabilitacao = true,
                NumeroHabilitacao = "98765432109",
                CategoriasHabilitacao = CategoriaHabilitacao.B | CategoriaHabilitacao.C,
                VeiculoId = null
            }
        };

        foreach (var usuario in usuariosSeed)
        {
            _usuarios[usuario.Id] = usuario;
        }
    }

    public Task<IReadOnlyCollection<Usuario>> ObterTodosAsync()
    {
        var usuarios = _usuarios.Values
            .OrderBy(u => u.Nome)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<Usuario>>(usuarios);
    }

    public Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        _usuarios.TryGetValue(id, out var usuario);
        return Task.FromResult(usuario);
    }

    public Task<Usuario> AdicionarAsync(Usuario usuario)
    {
        if (usuario.Id == Guid.Empty)
        {
            usuario.Id = Guid.NewGuid();
        }

        _usuarios[usuario.Id] = usuario;
        return Task.FromResult(usuario);
    }

    public Task<Usuario?> AtualizarAsync(Guid id, Usuario usuario)
    {
        if (!_usuarios.ContainsKey(id))
        {
            return Task.FromResult<Usuario?>(null);
        }

        usuario.Id = id;
        _usuarios[id] = usuario;

        return Task.FromResult<Usuario?>(usuario);
    }

    public Task<bool> RemoverAsync(Guid id)
    {
        return Task.FromResult(_usuarios.TryRemove(id, out _));
    }
}
