using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories;
using FluentAssertions;

namespace ControleDeFrota.Tests;

public class InMemoryUsuarioRepositoryTests
{
    [Fact]
    public async Task ObterTodosAsync_DeveRetornarUsuariosSeedados()
    {
        var repository = new InMemoryUsuarioRepository();

        var usuarios = await repository.ObterTodosAsync();

        usuarios.Should().NotBeEmpty();
        usuarios.Should().Contain(usuario => usuario.Email == "maria@empresa.com");
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
            Ativo = true,
            TipoContratacao = TipoContratacao.Funcionario,
            NumeroHabilitacao = "12345678900",
            CategoriasHabilitacao = CategoriaHabilitacao.B
        };

        await repository.AdicionarAsync(usuario);

        var resultado = await repository.ObterPorIdAsync(usuario.Id);

        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Ana Martins");
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
            Ativo = true,
            TipoContratacao = TipoContratacao.Funcionario,
            NumeroHabilitacao = "11122233344",
            CategoriasHabilitacao = CategoriaHabilitacao.BEC
        };

        await repository.AdicionarAsync(usuario);
        usuario.Nome = "Carlos Costa Jr.";

        var atualizado = await repository.AtualizarAsync(usuario.Id, usuario);

        atualizado.Should().NotBeNull();
        atualizado!.Nome.Should().Be("Carlos Costa Jr.");
    }
}
