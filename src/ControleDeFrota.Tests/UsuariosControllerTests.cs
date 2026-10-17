using ControleDeFrota.Api.Controllers;
using ControleDeFrota.Api.Dtos;
using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeFrota.Tests;

/// <summary>
/// Valida os comportamentos do controlador de usuários e a sincronização do vínculo entre usuários e veículos.
/// </summary>
public class UsuariosControllerTests
{
    /// <summary>
    /// Valida que a listagem retorna todos os usuários cadastrados.
    /// </summary>
    [Fact]
    public async Task Get_DeveRetornarTodosUsuarios()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new UsuariosController(usuarioRepository, veiculoRepository);

        var result = await controller.Get();

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeAssignableTo<IEnumerable<UsuarioResponseDto>>();
        ((IEnumerable<UsuarioResponseDto>)okResult.Value!).Should().NotBeEmpty();
    }

    /// <summary>
    /// Valida que a busca por identificador retorna um usuário existente.
    /// </summary>
    [Fact]
    public async Task GetById_DeveRetornarUsuario_QuandoUsuarioExistir()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new UsuariosController(usuarioRepository, veiculoRepository);

        var usuario = new Usuario
        {
            Nome = "Carlos Mendes",
            Email = "carlos@empresa.com",
            Departamento = "Operações",
            Ativo = true,
            TipoContratacao = TipoContratacao.Funcionario,
            NumeroHabilitacao = "11122233344",
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = null
        };
        await usuarioRepository.AdicionarAsync(usuario);

        var result = await controller.GetById(usuario.Id);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<UsuarioResponseDto>();
        var dto = okResult.Value as UsuarioResponseDto;
        dto!.Id.Should().Be(usuario.Id);
        dto.Nome.Should().Be(usuario.Nome);
    }

    /// <summary>
    /// Valida que o cadastro de usuário com vínculo a veículo sincroniza o relacionamento no veículo.
    /// </summary>
    [Fact]
    public async Task Post_DeveCadastrarUsuarioEAtualizarVinculoDoVeiculo()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new UsuariosController(usuarioRepository, veiculoRepository);

        var veiculo = new Veiculo
        {
            Placa = "XYZ-9876",
            Modelo = "Toyota Corolla",
            Ano = 2023,
            Cor = "Azul",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B
        };
        await veiculoRepository.AdicionarAsync(veiculo);

        var request = new UsuarioRequestDto
        {
            Nome = "Patricia Lima",
            Email = "patricia@empresa.com",
            Departamento = "Financeiro",
            Ativo = true,
            TipoContratacao = TipoContratacao.Funcionario,
            NumeroHabilitacao = "99887766554",
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = veiculo.Id
        };

        var result = await controller.Post(request);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var created = result.Result as CreatedAtActionResult;
        created!.ActionName.Should().Be(nameof(UsuariosController.GetById));
        created.Value.Should().BeOfType<UsuarioResponseDto>();

        var usuarioCriado = created.Value as UsuarioResponseDto;
        usuarioCriado.Should().NotBeNull();
        usuarioCriado!.VeiculoId.Should().Be(veiculo.Id);

        var veiculoAtualizado = await veiculoRepository.ObterPorIdAsync(veiculo.Id);
        veiculoAtualizado.Should().NotBeNull();
        veiculoAtualizado!.UsuarioId.Should().Be(usuarioCriado.Id);
    }

    /// <summary>
    /// Valida que a atualização de usuário mantém o vínculo com o veículo consistente em ambos os lados.
    /// </summary>
    [Fact]
    public async Task Put_DeveAtualizarUsuarioEManterVinculoComVeiculo()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new UsuariosController(usuarioRepository, veiculoRepository);

        var veiculo = new Veiculo
        {
            Placa = "OPQ-1234",
            Modelo = "Ford Ka",
            Ano = 2021,
            Cor = "Vermelho",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B
        };
        await veiculoRepository.AdicionarAsync(veiculo);

        var usuario = new Usuario
        {
            Nome = "Rafael Costa",
            Email = "rafael@empresa.com",
            Departamento = "Logística",
            Ativo = true,
            TipoContratacao = TipoContratacao.Funcionario,
            NumeroHabilitacao = "44433322211",
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = null
        };
        await usuarioRepository.AdicionarAsync(usuario);

        var request = new UsuarioRequestDto
        {
            Nome = usuario.Nome,
            Email = usuario.Email,
            Departamento = usuario.Departamento,
            Ativo = usuario.Ativo,
            TipoContratacao = usuario.TipoContratacao,
            NumeroHabilitacao = usuario.NumeroHabilitacao,
            CategoriasHabilitacao = usuario.CategoriasHabilitacao,
            VeiculoId = veiculo.Id
        };

        var result = await controller.Put(usuario.Id, request);

        result.Should().BeOfType<NoContentResult>();

        var usuarioAtualizado = await usuarioRepository.ObterPorIdAsync(usuario.Id);
        usuarioAtualizado.Should().NotBeNull();
        usuarioAtualizado!.VeiculoId.Should().Be(veiculo.Id);

        var veiculoAtualizado = await veiculoRepository.ObterPorIdAsync(veiculo.Id);
        veiculoAtualizado.Should().NotBeNull();
        veiculoAtualizado!.UsuarioId.Should().Be(usuario.Id);
    }

    /// <summary>
    /// Valida que a remoção de um usuário limpa o vínculo do veículo associado.
    /// </summary>
    [Fact]
    public async Task Delete_DeveRemoverUsuarioELimparVinculoDoVeiculo()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new UsuariosController(usuarioRepository, veiculoRepository);

        var usuario = new Usuario
        {
            Nome = "Fernanda Rocha",
            Email = "fernanda@empresa.com",
            Departamento = "TI",
            Ativo = true,
            TipoContratacao = TipoContratacao.Funcionario,
            NumeroHabilitacao = "55544433322",
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = null
        };
        await usuarioRepository.AdicionarAsync(usuario);

        var veiculo = new Veiculo
        {
            Placa = "LMN-8877",
            Modelo = "Hyundai Creta",
            Ano = 2024,
            Cor = "Branco",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B,
            UsuarioId = usuario.Id
        };
        await veiculoRepository.AdicionarAsync(veiculo);

        usuario.VeiculoId = veiculo.Id;
        await usuarioRepository.AtualizarAsync(usuario.Id, usuario);

        var result = await controller.Delete(usuario.Id);

        result.Should().BeOfType<NoContentResult>();

        var usuarioRemovido = await usuarioRepository.ObterPorIdAsync(usuario.Id);
        usuarioRemovido.Should().BeNull();

        var veiculoAtualizado = await veiculoRepository.ObterPorIdAsync(veiculo.Id);
        veiculoAtualizado.Should().NotBeNull();
        veiculoAtualizado!.UsuarioId.Should().BeNull();
    }

    /// <summary>
    /// Valida que a remoção de um usuário inexistente retorna not found.
    /// </summary>
    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new UsuariosController(usuarioRepository, veiculoRepository);

        var result = await controller.Delete(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }
}
