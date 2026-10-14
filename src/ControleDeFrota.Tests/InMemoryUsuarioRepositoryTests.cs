using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories;

namespace ControleDeFrota.Tests;

public class InMemoryUsuarioRepositoryTests
{
    [Fact]
    public async Task ObterTodosAsync_DeveRetornarUsuariosSeedados()
    {
        var repository = new InMemoryUsuarioRepository();

        var usuarios = await repository.ObterTodosAsync();

        Assert.NotEmpty(usuarios);
        Assert.Contains(usuarios, usuario => usuario.Email == "maria@empresa.com");
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarNovoUsuario()
    {
        var repository = new InMemoryUsuarioRepository();
        var usuario = new Usuario
        {
            Nome = "Ana Martins",
            Email = "ana@empresa.com",
            Departamento = "Financeiro",
            Ativo = true
        };

        await repository.AdicionarAsync(usuario);

        var resultado = await repository.ObterPorIdAsync(usuario.Id);

        Assert.NotNull(resultado);
        Assert.Equal("Ana Martins", resultado!.Nome);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarUsuarioExistente()
    {
        var repository = new InMemoryUsuarioRepository();
        var usuario = new Usuario
        {
            Nome = "Carlos Costa",
            Email = "carlos@empresa.com",
            Departamento = "TI",
            Ativo = true
        };

        await repository.AdicionarAsync(usuario);
        usuario.Nome = "Carlos Costa Jr.";

        var atualizado = await repository.AtualizarAsync(usuario.Id, usuario);

        Assert.NotNull(atualizado);
        Assert.Equal("Carlos Costa Jr.", atualizado!.Nome);
    }
}
