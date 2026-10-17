using ControleDeFrota.Api.Controllers;
using ControleDeFrota.Api.Dtos;
using ControleDeFrota.Api.Models;
using ControleDeFrota.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeFrota.Tests;

public class VeiculosControllerIntegrationTests
{
    /// <summary>
    /// Valida que a listagem retorna todos os veículos cadastrados.
    /// </summary>
    [Fact]
    public async Task Get_DeveRetornarTodosVeiculos()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new VeiculosController(veiculoRepository, usuarioRepository);

        var veiculo = new Veiculo
        {
            Placa = "LMN-4567",
            Modelo = "Volkswagen Voyage",
            Ano = 2022,
            Cor = "Branco",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B
        };
        await veiculoRepository.AdicionarAsync(veiculo);

        var result = await controller.Get();

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeAssignableTo<IEnumerable<VeiculoResponseDto>>();
        ((IEnumerable<VeiculoResponseDto>)okResult.Value!).Should().Contain(v => v.Id == veiculo.Id);
    }

    /// <summary>
    /// Valida que a busca por identificador retorna o veículo solicitado.
    /// </summary>
    [Fact]
    public async Task GetById_DeveRetornarVeiculo_QuandoVeiculoExistir()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new VeiculosController(veiculoRepository, usuarioRepository);

        var veiculo = new Veiculo
        {
            Placa = "QWE-1010",
            Modelo = "Fiat Strada",
            Ano = 2023,
            Cor = "Preta",
            Disponivel = false,
            Categoria = CategoriaHabilitacao.B
        };
        await veiculoRepository.AdicionarAsync(veiculo);

        var result = await controller.GetById(veiculo.Id);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<VeiculoResponseDto>();
        var dto = okResult.Value as VeiculoResponseDto;
        dto!.Id.Should().Be(veiculo.Id);
        dto.Placa.Should().Be(veiculo.Placa);
    }

    /// <summary>
    /// Valida que o veículo pode ser cadastrado sem informar o usuário no payload do POST.
    /// </summary>
    [Fact]
    public async Task Post_DeveCadastrarVeiculo()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new VeiculosController(veiculoRepository, usuarioRepository);

        var request = new VeiculoRequestDto
        {
            Placa = "XYZ-2024",
            Modelo = "Honda Fit",
            Ano = 2024,
            Cor = "Prata",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B
        };

        var result = await controller.Post(request);

        result.Result.Should().BeOfType<CreatedAtActionResult>();

        var created = result.Result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.RouteValues.Should().ContainKey("id");

        var veiculoCriado = created.Value as VeiculoResponseDto;
        veiculoCriado.Should().NotBeNull();
        veiculoCriado!.UsuarioId.Should().BeNull();

        var veiculoSalvo = await veiculoRepository.ObterPorIdAsync(veiculoCriado.Id);
        veiculoSalvo.Should().NotBeNull();
        veiculoSalvo!.UsuarioId.Should().BeNull();
    }

    /// <summary>
    /// Valida que o vínculo do usuário com o veículo é atualizado no PUT quando o usuário informado é válido.
    /// </summary>
    [Fact]
    public async Task Put_DeveVincularUsuarioAoVeiculo_QuandoUsuarioForValido()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new VeiculosController(veiculoRepository, usuarioRepository);

        var usuario = new Usuario
        {
            Nome = "João da Silva",
            Email = "joao@empresa.com",
            Departamento = "Operações",
            Ativo = true,
            TipoContratacao = TipoContratacao.Funcionario,
            NumeroHabilitacao = "12345678901",
            CategoriasHabilitacao = CategoriaHabilitacao.B
        };
        await usuarioRepository.AdicionarAsync(usuario);

        var veiculo = new Veiculo
        {
            Placa = "ABC-9999",
            Modelo = "Fiat Palio",
            Ano = 2024,
            Cor = "Branco",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B
        };
        await veiculoRepository.AdicionarAsync(veiculo);

        var request = new VeiculoUpdateRequestDto
        {
            Placa = veiculo.Placa,
            Modelo = veiculo.Modelo,
            Ano = veiculo.Ano,
            Cor = veiculo.Cor,
            Disponivel = veiculo.Disponivel,
            Categoria = veiculo.Categoria,
            UsuarioId = usuario.Id
        };

        var result = await controller.Put(veiculo.Id, request);

        result.Should().BeOfType<NoContentResult>();

        var veiculoAtualizado = await veiculoRepository.ObterPorIdAsync(veiculo.Id);
        veiculoAtualizado.Should().NotBeNull();
        veiculoAtualizado!.UsuarioId.Should().Be(usuario.Id);

        var usuarioAtualizado = await usuarioRepository.ObterPorIdAsync(usuario.Id);
        usuarioAtualizado.Should().NotBeNull();
        usuarioAtualizado!.VeiculoId.Should().Be(veiculo.Id);
    }

    /// <summary>
    /// Valida que o PUT rejeita um usuário inexistente no payload.
    /// </summary>
    [Fact]
    public async Task Put_DeveRetornarBadRequest_QuandoUsuarioInformadoNaoExiste()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var controller = new VeiculosController(veiculoRepository, usuarioRepository);

        var veiculo = new Veiculo
        {
            Placa = "TTT-0001",
            Modelo = "Chevrolet Prisma",
            Ano = 2023,
            Cor = "Azul",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B
        };
        await veiculoRepository.AdicionarAsync(veiculo);

        var request = new VeiculoUpdateRequestDto
        {
            Placa = veiculo.Placa,
            Modelo = veiculo.Modelo,
            Ano = veiculo.Ano,
            Cor = veiculo.Cor,
            Disponivel = veiculo.Disponivel,
            Categoria = veiculo.Categoria,
            UsuarioId = Guid.NewGuid()
        };

        var result = await controller.Put(veiculo.Id, request);

        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().Be("Usuário informado não foi encontrado.");
    }

    /// <summary>
    /// Valida que ao remover um usuário, o veículo vinculado perde o referência do usuário.
    /// </summary>
    [Fact]
    public async Task Delete_DeveRemoverVinculoDoVeiculo_QuandoUsuarioForRemovido()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var usuarioController = new UsuariosController(usuarioRepository, veiculoRepository);

        var usuario = new Usuario
        {
            Nome = "Ana Maria",
            Email = "ana@empresa.com",
            Departamento = "Financeiro",
            Ativo = true,
            TipoContratacao = TipoContratacao.Funcionario,
            NumeroHabilitacao = "22233344455",
            CategoriasHabilitacao = CategoriaHabilitacao.B
        };
        await usuarioRepository.AdicionarAsync(usuario);

        var veiculo = new Veiculo
        {
            Placa = "DDD-1111",
            Modelo = "Renault Logan",
            Ano = 2025,
            Cor = "Cinza",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B,
            UsuarioId = usuario.Id
        };
        await veiculoRepository.AdicionarAsync(veiculo);

        usuario.VeiculoId = veiculo.Id;
        await usuarioRepository.AtualizarAsync(usuario.Id, usuario);

        var result = await usuarioController.Delete(usuario.Id);

        result.Should().BeOfType<NoContentResult>();

        var veiculoAtualizado = await veiculoRepository.ObterPorIdAsync(veiculo.Id);
        veiculoAtualizado.Should().NotBeNull();
        veiculoAtualizado!.UsuarioId.Should().BeNull();

        var usuarioRemovido = await usuarioRepository.ObterPorIdAsync(usuario.Id);
        usuarioRemovido.Should().BeNull();
    }

    /// <summary>
    /// Valida que ao remover um veículo, o usuário vinculado perde a referência do veículo.
    /// </summary>
    [Fact]
    public async Task Delete_DeveRemoverVinculoDoUsuario_QuandoVeiculoForRemovido()
    {
        var usuarioRepository = new InMemoryUsuarioRepository();
        var veiculoRepository = new InMemoryVeiculoRepository();
        var veiculoController = new VeiculosController(veiculoRepository, usuarioRepository);

        var usuario = new Usuario
        {
            Nome = "Bruno Souza",
            Email = "bruno@empresa.com",
            Departamento = "TI",
            Ativo = true,
            TipoContratacao = TipoContratacao.Funcionario,
            NumeroHabilitacao = "55566677788",
            CategoriasHabilitacao = CategoriaHabilitacao.B,
            VeiculoId = Guid.Empty
        };
        await usuarioRepository.AdicionarAsync(usuario);

        var veiculo = new Veiculo
        {
            Placa = "EEE-2222",
            Modelo = "Volkswagen Gol",
            Ano = 2024,
            Cor = "Preto",
            Disponivel = true,
            Categoria = CategoriaHabilitacao.B,
            UsuarioId = usuario.Id
        };
        await veiculoRepository.AdicionarAsync(veiculo);

        usuario.VeiculoId = veiculo.Id;
        await usuarioRepository.AtualizarAsync(usuario.Id, usuario);

        var result = await veiculoController.Delete(veiculo.Id);

        result.Should().BeOfType<NoContentResult>();

        var usuarioAtualizado = await usuarioRepository.ObterPorIdAsync(usuario.Id);
        usuarioAtualizado.Should().NotBeNull();
        usuarioAtualizado!.VeiculoId.Should().BeNull();

        var veiculoRemovido = await veiculoRepository.ObterPorIdAsync(veiculo.Id);
        veiculoRemovido.Should().BeNull();
    }
}
